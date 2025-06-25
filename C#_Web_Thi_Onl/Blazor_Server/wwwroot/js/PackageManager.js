let answerCount = 3;

function addAnswer() {
    answerCount++;
    const container = document.getElementById("answer-container");

    const newAnswer = document.createElement("div");
    newAnswer.className = "input-group mb-2";

    newAnswer.innerHTML = `
            <span class="input-group-text">
                <input type="checkbox" @bind="ans.Right"/>
            </span>
            <input class="form-control" type="text" placeholder="Đáp án ${answerCount}" @bind="ans.Name/>
            <button class="btn btn-danger" onclick="removeAnswer(this)">❌</button>
        `;

    container.appendChild(newAnswer);
}

function removeAnswer(button) {
    const inputGroup = button.closest(".input-group");
    if (inputGroup) {
        inputGroup.remove();
    }
}

let singleAnswerCount = 3;

function addSingleAnswer() {
    singleAnswerCount++;

    const container = document.getElementById("single-answer-container");

    const newAnswer = document.createElement("div");
    newAnswer.className = "input-group mb-2";

    newAnswer.innerHTML = `
            <span class="input-group-text">
                <input type="radio" name="singleanswer" @bind="ans.Right"/>
            </span>
            <input class="form-control" type="text" placeholder="Đáp án ${singleAnswerCount}" @bind="ans.Name/>
            <button class="btn btn-danger" onclick="removeSingleAnswer(this)">❌</button>
        `;

    container.appendChild(newAnswer);
}

function removeSingleAnswer(button) {
    const inputGroup = button.closest(".input-group");
    if (inputGroup) {
        inputGroup.remove();
    }
}