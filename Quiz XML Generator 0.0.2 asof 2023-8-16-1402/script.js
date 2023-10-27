// Global variables to store quiz data
let numQuestions = 0;
let quizData = [];

// Add this code to initialize IndexedDB
let db;
const request = indexedDB.open("QuizDB", 1);

request.onerror = function(event) {
  console.error("IndexedDB error:", event.target.error);
};

request.onsuccess = function(event) {
  db = event.target.result;
  console.log("IndexedDB opened successfully");
};

request.onupgradeneeded = function(event) {
  db = event.target.result;
  const objectStore = db.createObjectStore("quizStore", { keyPath: "quizId", autoIncrement: true });
};

// Function to save quiz data to IndexedDB
function saveQuizToDB(quizName, data) {
  const transaction = db.transaction(["quizStore"], "readwrite");
  const objectStore = transaction.objectStore("quizStore");

  const request = objectStore.add({ quizName: quizName, data: data });

  request.onsuccess = function(event) {
    console.log("Data saved to IndexedDB");
  };

  request.onerror = function(event) {
    console.error("Error saving data to IndexedDB:", event.target.error);
  };
}

// Function to retrieve quiz data from IndexedDB
function loadQuizFromDB(quizId) {
  const transaction = db.transaction(["quizStore"], "readonly");
  const objectStore = transaction.objectStore("quizStore");

  const request = objectStore.get(quizId);

  request.onsuccess = function(event) {
    const quizData = event.target.result;
    // Populate your quiz interface with the retrieved data here
  };

  request.onerror = function(event) {
    console.error("Error loading data from IndexedDB:", event.target.error);
  };
}

// Function to add a new question
function addQuestion() {
  const numChoicesInput = document.getElementById("numChoices");
  const numChoices = parseInt(numChoicesInput.value);

  if (isNaN(numChoices) || numChoices < 2) {
    alert("Please enter a valid number of choices.");
    return;
  }

  const quizContainer = document.getElementById("quizContainer");

  const questionDiv = document.createElement("div");
  questionDiv.className = "question";

  const questionLabel = document.createElement("label");
  questionLabel.textContent = `Question ${numQuestions + 1}: `;
  const questionInput = document.createElement("input");
  questionInput.type = "text";
  questionInput.className = "question-input required-field";
  questionInput.placeholder = "Enter the question";
  questionDiv.appendChild(questionLabel);
  questionDiv.appendChild(questionInput);
  questionDiv.appendChild(document.createElement("br"));

  const options = [];
  for (let j = 1; j <= numChoices; j++) {
    const choiceLabel = document.createElement("label");
    choiceLabel.textContent = `Choice ${j}: `;
    const choiceInput = document.createElement("input");
    choiceInput.type = "text";
    choiceInput.className = "choice-input required-field";
    choiceInput.placeholder = "Enter choice";
    questionDiv.appendChild(choiceLabel);
    questionDiv.appendChild(choiceInput);
    questionDiv.appendChild(document.createElement("br"));
    options.push({ text: "" });
  }

  const correctAnswerLabel = document.createElement("label");
  correctAnswerLabel.textContent = `Correct Answer Choice (1-${numChoices}): `;
  const correctAnswerInput = document.createElement("input");
  correctAnswerInput.type = "number";
  correctAnswerInput.min = 1;
  correctAnswerInput.max = numChoices;
  correctAnswerInput.className = "correct-answer-input required-field";
  questionDiv.appendChild(correctAnswerLabel);
  questionDiv.appendChild(correctAnswerInput);
  questionDiv.appendChild(document.createElement("br"));

  const correctExplainLabel = document.createElement("label");
  correctExplainLabel.textContent = "Explanation for Correct Answer: ";
  const correctExplainInput = document.createElement("input");
  correctExplainInput.type = "text";
  correctExplainInput.className = "correct-explain-input required-field";
  correctExplainInput.placeholder = "Enter explanation for correct answer";
  questionDiv.appendChild(correctExplainLabel);
  questionDiv.appendChild(correctExplainInput);
  questionDiv.appendChild(document.createElement("br"));

  const incorrectExplainLabel = document.createElement("label");
  incorrectExplainLabel.textContent = "Explanation for Incorrect Answer: ";
  const incorrectExplainInput = document.createElement("input");
  incorrectExplainInput.type = "text";
  incorrectExplainInput.className = "incorrect-explain-input required-field";
  incorrectExplainInput.placeholder = "Enter explanation for incorrect answer";
  questionDiv.appendChild(incorrectExplainLabel);
  questionDiv.appendChild(incorrectExplainInput);
  questionDiv.appendChild(document.createElement("br"));

  // Add Delete button
  const deleteButton = document.createElement("button");
  deleteButton.textContent = "Delete";
  deleteButton.className = "delete-button";
  deleteButton.addEventListener("click", () => deleteQuestion(numQuestions));
  questionDiv.appendChild(deleteButton);

  // Add Clear button
  const clearButton = document.createElement("button")
  clearButton.textContent = "Clear"; 
  clearButton.className = "clear-button"
  clearButton.addEventListener("click",() => clearQuestionContainer(questionDiv));
  questionDiv.append(clearButton)

  quizContainer.appendChild(questionDiv);

  // Increment the question count
  numQuestions++;

  // Update the question count label
  updateQuestionCountLabel();

  // Enable the save and copy buttons
  document.getElementById("saveButton").disabled = false;
  document.getElementById("copyButton").disabled = false;
}


// Function to update the question count label
function updateQuestionCountLabel() {
  const questionCountLabel = document.getElementById("questionCountLabel");
  questionCountLabel.textContent = `Number of Questions: ${numQuestions}`;
}

// Function to delete a question
function deleteQuestion(questionIndex) {
  const quizContainer = document.getElementById("quizContainer");
  const questionDivs = quizContainer.getElementsByClassName("question");

  if (questionIndex < 1 || questionIndex > numQuestions || questionDivs.length === 0) {
    alert("Invalid question index.");
    return;
  }

  // Get the question container to be removed
  const questionContainer = questionDivs[questionIndex - 1];

  // Remove the question from the quizData array
  quizData.splice(questionIndex - 1, 1);

  // Remove the question container from the display
  questionContainer.remove();

  // Update the question count
  numQuestions--;

  // Update the question count label
  updateQuestionCountLabel();

  // Disable the save and copy buttons if there are no questions
  if (numQuestions === 0) {
    document.getElementById("saveButton").disabled = true;
    document.getElementById("copyButton").disabled = true;
  }
}

// Function to clear all input fields in a Question Container
function clearQuestionContainer(container) {
  const inputs = container.querySelectorAll('input');
  
  inputs.forEach(input => {
    input.value = '';
    input.classList.remove('invalid'); // Remove any validation error styling
  });
}

// Function to check if all required fields are filled
function validateInputs() {
  const requiredFields = document.getElementsByClassName("required-field");
  let isValid = true;

  for (const field of requiredFields) {
    if (field.value.trim() === "") {
      field.classList.add("invalid");
      isValid = false;
    } else {
      field.classList.remove("invalid");
    }
  }

  return isValid;
}

// Function to retrieve quiz data from IndexedDB
function loadQuizFromDB(quizId) {
  const transaction = db.transaction(["quizStore"], "readonly");
  const objectStore = transaction.objectStore("quizStore");

  const request = objectStore.get(quizId);

  request.onsuccess = function(event) {
    const quizData = event.target.result;
    // Call populateQuizInterface here with the retrieved quizData
    populateQuizInterface(quizData.data);
  };

  request.onerror = function(event) {
    console.error("Error loading data from IndexedDB:", event.target.error);
  };
}

// Update your loadQuiz function to populate the interface
// when the Load button is clicked
function loadQuiz(quizId) {
  const quizContainer = document.getElementById("quizContainer");
  
  // Clear existing questions by clearing the quiz container
  clearQuestionContainer(quizContainer);

  // Load quiz data from IndexedDB and populate the interface
  loadQuizFromDB(quizId);
}


function populateQuizInterface(loadedData) {
  const quizContainer = document.getElementById("quizContainer");

  // Clear existing questions by clearing the quiz container
  clearQuestionContainer(quizContainer);

  // Iterate through the loaded quiz data and populate the interface
  loadedData.forEach((item) => {
    // Create a new question container
    const questionDiv = document.createElement("div");
    questionDiv.className = "question";

    // Populate the question and other inputs
    const questionLabel = document.createElement("label");
    questionLabel.textContent = `Question ${numQuestions + 1}: `;
    const questionInput = document.createElement("input");
    questionInput.type = "text";
    questionInput.className = "question-input required-field";
    questionInput.value = item.question;
    questionDiv.appendChild(questionLabel);
    questionDiv.appendChild(questionInput);
    questionDiv.appendChild(document.createElement("br"));

    // Populate options
    item.options.forEach((option, optionIndex) => {
      const choiceLabel = document.createElement("label");
      choiceLabel.textContent = `Choice ${optionIndex + 1}: `;
      const choiceInput = document.createElement("input");
      choiceInput.type = "text";
      choiceInput.className = "choice-input required-field";
      choiceInput.value = option.text;
      questionDiv.appendChild(choiceLabel);
      questionDiv.appendChild(choiceInput);
      questionDiv.appendChild(document.createElement("br"));
    });

    // Populate correct answer choice
    const correctAnswerLabel = document.createElement("label");
    correctAnswerLabel.textContent = `Correct Answer Choice (1-${item.options.length}): `;
    const correctAnswerInput = document.createElement("input");
    correctAnswerInput.type = "number";
    correctAnswerInput.min = 1;
    correctAnswerInput.max = item.options.length;
    correctAnswerInput.className = "correct-answer-input required-field";
    correctAnswerInput.value = item.answer + 1;
    questionDiv.appendChild(correctAnswerLabel);
    questionDiv.appendChild(correctAnswerInput);
    questionDiv.appendChild(document.createElement("br"));

    // Populate explanations
    const correctExplainLabel = document.createElement("label");
    correctExplainLabel.textContent = "Explanation for Correct Answer: ";
    const correctExplainInput = document.createElement("input");
    correctExplainInput.type = "text";
    correctExplainInput.className = "correct-explain-input required-field";
    correctExplainInput.value = item.correctExplain;
    questionDiv.appendChild(correctExplainLabel);
    questionDiv.appendChild(correctExplainInput);
    questionDiv.appendChild(document.createElement("br"));

    const incorrectExplainLabel = document.createElement("label");
    incorrectExplainLabel.textContent = "Explanation for Incorrect Answer: ";
    const incorrectExplainInput = document.createElement("input");
    incorrectExplainInput.type = "text";
    incorrectExplainInput.className = "incorrect-explain-input required-field";
    incorrectExplainInput.value = item.incorrectExplain;
    questionDiv.appendChild(incorrectExplainLabel);
    questionDiv.appendChild(incorrectExplainInput);
    questionDiv.appendChild(document.createElement("br"));

    // Add Delete button
    const deleteButton = document.createElement("button");
    deleteButton.textContent = "Delete";
    deleteButton.className = "delete-button";
    deleteButton.addEventListener("click", () => deleteQuestion(numQuestions));
    questionDiv.appendChild(deleteButton);

    // Add Clear button
    const clearButton = document.createElement("button");
    clearButton.textContent = "Clear";
    clearButton.className = "clear-button";
    clearButton.addEventListener("click", () => clearQuestionContainer(questionDiv));
    questionDiv.appendChild(clearButton);

    // Append the question container to the quiz container
    quizContainer.appendChild(questionDiv);

    // Increment the question count
    numQuestions++;
  });

  // Update the question count label
  updateQuestionCountLabel();

  // Enable the save and copy buttons
  document.getElementById("saveButton").disabled = false;
  document.getElementById("copyButton").disabled = false;
}


// Function to save the quiz data as XML
function saveQuizAsXML() {
  if (!validateInputs()) {
    alert("Please fill in all required fields before saving.");
    return;
  }

  const quizContainer = document.getElementById("quizContainer");
  const questionDivs = quizContainer.getElementsByClassName("question");

  quizData = [];
  for (let i = 0; i < questionDivs.length; i++) {
    const questionDiv = questionDivs[i];

    const questionInput = questionDiv.querySelector(".question-input");
    const choiceInputs = questionDiv.querySelectorAll(".choice-input");
    const correctAnswerInput = questionDiv.querySelector(".correct-answer-input");
    const correctExplainInput = questionDiv.querySelector(".correct-explain-input");
    const incorrectExplainInput = questionDiv.querySelector(".incorrect-explain-input");

    const question = questionInput.value.trim();

    const options = [];
    for (let j = 0; j < choiceInputs.length; j++) {
      const choice = choiceInputs[j].value.trim();
      options.push({ text: choice });
    }

    const correctAnswerIndex = parseInt(correctAnswerInput.value);
    const correctAnswer = isNaN(correctAnswerIndex) ? 1 : correctAnswerIndex - 1;

    const correctExplain = correctExplainInput.value.trim();
    const incorrectExplain = incorrectExplainInput.value.trim();

    quizData.push({
      question: question,
      options: options,
      answer: correctAnswer,
      correctExplain: correctExplain,
      incorrectExplain: incorrectExplain,
    });
  }

  // Convert quizData to XML format
  const xmlString = convertToXML(quizData);

  // Set the XML data in the textarea
  const xmlTextArea = document.getElementById("xmlTextArea");
  xmlTextArea.value = xmlString;

  // Create a Blob with the XML data
  const blob = new Blob([xmlString], { type: "text/xml" });

  // Create a link to download the Blob
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;

  const quizName = prompt("Enter a name for the quiz:", "Quiz");
  if (quizName) {
    // Save quizData to IndexedDB with the dynamic quizName
    saveQuizToDB(quizName, quizData);
  }

  // Prompt the user to enter the filename
  const filename = prompt("Enter the filename for the XML file:", "quiz.xml");
  if (filename) {
    link.download = filename;
    link.click();
  } else {
    alert("Invalid filename. Please enter a valid filename.");
  }

  // Clean up the URL and remove the link element
  URL.revokeObjectURL(url);
  link.remove();
}


function copyXmlToClipboard() {
  const xmlTextArea = document.getElementById("xmlTextArea");
  xmlTextArea.select();
  document.execCommand("copy");
  alert("XML copied to clipboard!");
}

// Function to set an input element as invalids
function setInvalidInput(inputElement) {
  inputElement.classList.add("error");
}

// Function to convert quizData to XML format
function convertToXML(data) {
  let xmlString = `<?xml version="1.0" encoding="UTF-8"?>\n<quiz score="${numQuestions}" answerstyle="none">\n`;



  data.forEach((item, index) => {
    xmlString += `  <question index="${index + 1}" text="${item.question}" answer="${item.answer + 1}">\n`;

    // Add options
    item.options.forEach((option, optionIndex) => {
      xmlString += `    <option i="${optionIndex + 1}" text="${option.text}" />\n`;
    });

    // Add explanations
    xmlString += `    <explanation text="${item.correctExplain}" appearance="correct" />\n`;
    xmlString += `    <explanation text="${item.incorrectExplain}" appearance="incorrect" />\n`;

    xmlString += '  </question>\n';
  });

  xmlString += "</quiz>";

  return xmlString;
}


// Event listener for Add Question button
document.getElementById("addQuestionButton").addEventListener("click", addQuestion);

// Event listeners for buttons
document.getElementById("saveButton").addEventListener("click", saveQuizAsXML);
document.getElementById("copyButton").addEventListener("click", copyXmlToClipboard);

// Event listener for the Load button
document.getElementById("loadButton").addEventListener("click", function() {
  // Prompt the user for a quizId
  const quizId = parseInt(prompt("Enter the quiz id to load(i.e: 1):"));

  if (!isNaN(quizId)) {
    loadQuiz(quizId);
  } else {
    alert("Invalid quiz id. Please enter a valid number.");
  }
});
