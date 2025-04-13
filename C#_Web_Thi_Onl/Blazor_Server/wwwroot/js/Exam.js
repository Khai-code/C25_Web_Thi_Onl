function showAnswerFields() {
    const questionType = document.getElementById("questionType").value;
    const answerFields = document.getElementById("answerFields");
    const answersContainer = document.getElementById("answersContainer");
    const addAnswerBtn = document.getElementById("addAnswerBtn");

    answersContainer.innerHTML = "";
    addAnswerBtn.style.display = "none";

    if (questionType === "1" || questionType === "2") {
        for (let i = 1; i <= 4; i++) {
            addAnswerField(i, questionType);
        }
        answerFields.style.display = "block";
        addAnswerBtn.style.display = "inline-block";
    } else if (questionType === "3") {
        answersContainer.innerHTML = `
                <div class="form-check">
                    <input class="form-check-input" type="radio" name="trueFalse" @bind="questionViewModel.Answers.Answers_Name" value="true" required>
                    <label class="form-check-label">Đúng</label>
                </div>
                <div class="form-check">
                    <input class="form-check-input" type="radio" name="trueFalse" @bind="questionViewModel.Answers.Answers_Name" value="false" required>
                    <label class="form-check-label">Sai</label>
                </div>
            `;
        answerFields.style.display = "block";
    } else {
        answerFields.style.display = "none";
    }
}

function addAnswerField(index, type) {
    const answersContainer = document.getElementById("answersContainer");
    const div = document.createElement("div");
    div.classList.add("form-check", "mt-2");
    div.innerHTML = `
            <input class="form-check-input" type="${type === '1' ? 'radio' : 'checkbox'}" name="answerOption">
            <input type="text" class="form-control d-inline w-75" @bind="questionViewModel.Answers.Answers_Name" placeholder="Nhập đáp án ${index}" required>
        `;
    answersContainer.appendChild(div);
}