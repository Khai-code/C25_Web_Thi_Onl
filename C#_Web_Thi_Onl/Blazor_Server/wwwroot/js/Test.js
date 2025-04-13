
    const questions = [
    {
        question: "Câu 1: Thủ đô của Việt Nam là gì?",
    options: ["Hà Nội", "TP.HCM", "Huế", "Đà Nẵng"]
      },
    {
        question: "Câu 2: 5 + 3 bằng bao nhiêu?",
    options: ["6", "7", "8", "9"]
      },
    {
        question: "Câu 3: Mặt trời mọc ở hướng nào?",
    options: ["Đông", "Tây", "Nam", "Bắc"]
      }
    ];

    let currentQuestion = 0;
    let answers = new Array(questions.length).fill(null);

    const questionContainer = document.getElementById("question-container");
    const prevBtn = document.getElementById("prev-btn");
    const nextBtn = document.getElementById("next-btn");
    const finishBtn = document.getElementById("finish-btn");
    const resultDiv = document.getElementById("result");

    function renderQuestion(index) {
      const q = questions[index];
    let html = `
    <div class="question-text"><strong>${q.question}</strong></div>
    <div class="options">
        `;
      q.options.forEach((opt, i) => {
        const checked = answers[index] === i ? "checked" : "";
        html += `
        <label>
            <input type="radio" name="answer" value="${i}" ${checked}> ${opt}
        </label>
        `;
      });
        html += `</div>`;
    questionContainer.innerHTML = html;

    prevBtn.disabled = index === 0;
    nextBtn.disabled = index === questions.length - 1;

      document.querySelectorAll('input[name="answer"]').forEach(radio => {
        radio.addEventListener("change", e => {
            answers[currentQuestion] = parseInt(e.target.value);
            checkAllAnswered();
        });
      });
    }

    function checkAllAnswered() {
      const allAnswered = answers.every(a => a !== null);
    finishBtn.style.display = allAnswered ? "block" : "none";
    }

    prevBtn.addEventListener("click", () => {
      if (currentQuestion > 0) {
        currentQuestion--;
    renderQuestion(currentQuestion);
      }
    });

    nextBtn.addEventListener("click", () => {
      if (currentQuestion < questions.length - 1) {
        currentQuestion++;
    renderQuestion(currentQuestion);
      }
    });

    finishBtn.addEventListener("click", () => {
        document.querySelector('.quiz-container').innerHTML = `
        <div id="result">
          <h2>🎉 Bạn đã hoàn thành bài thi!</h2>
          <p>Cảm ơn bạn đã tham gia.</p>
        </div>
      `;
    });

    // Khởi động
    renderQuestion(currentQuestion);
