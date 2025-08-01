﻿window.registerDotnetHelper = function (dotnetHelper) {
    window.dotnetHelper = dotnetHelper;
};

window.startPersistentFullscreen = function () {

    if (window.location.pathname !== "/Test") {
        console.log("Không phải trang /Test → bỏ qua fullscreen");
        return;
    }

    function goFullscreen() {
        const docEl = document.documentElement;
        if (docEl.requestFullscreen) {
            docEl.requestFullscreen();
        } else if (docEl.webkitRequestFullscreen) {
            docEl.webkitRequestFullscreen();
        } else if (docEl.mozRequestFullScreen) {
            docEl.mozRequestFullScreen();
        } else if (docEl.msRequestFullscreen) {
            docEl.msRequestFullscreen();
        }
    }

    goFullscreen();

    document.addEventListener("fullscreenchange", () => {
        if (!document.fullscreenElement) {
            console.log("Thoát fullscreen. Hiện modal yêu cầu vào lại.");
            showFullscreenModal(goFullscreen);
        }
    });

    // PHÁT HIỆN MẤT FOCUS
    let isWindowBlurred = false;
    let violationCount = 0;
    const maxViolations = 2;

    window.addEventListener('blur', function () {
        console.log("Mất focus (blur). Khả năng user vừa Alt+Tab hoặc rời tab.");
        isWindowBlurred = true;
    });

    window.addEventListener('focus', function () {
        if (isWindowBlurred) {
            isWindowBlurred = false;
            violationCount++;
            console.log("Phát hiện user quay lại sau khi blur. Lần vi phạm:", violationCount);

            if (violationCount > maxViolations) {
                console.log("Vượt quá số lần vi phạm cho phép → kick khỏi bài thi.");
                callResetTimeAndRedirect();
            } else {
                showWarningModal(`⚠️ Phát hiện bạn rời khỏi bài thi.<br>Lần vi phạm: ${violationCount}/${maxViolations}.<br>Nếu vượt quá giới hạn, bài thi sẽ kết thúc.`);
            }
        }
    });

    document.querySelectorAll("textarea, input").forEach(el => {
        el.addEventListener("paste", (e) => {
            alert("Không được dán nội dung vào bài thi!");
            e.preventDefault();
        });
    });

    // CHẶN PHÍM NGUY HIỂM
    document.addEventListener('keydown', function (event) {
        console.log("Phím:", event.key, "Code:", event.code, "Ctrl:", event.ctrlKey, "Alt:", event.altKey);

        if (event.key === "Control" || event.code === "ControlLeft" || event.code === "ControlRight") {
            console.log("Chặn phím Ctrl riêng.");
            event.preventDefault();
            event.stopPropagation();
            return;
        }

        if (event.ctrlKey) {
            console.log("Chặn tổ hợp Ctrl + phím khác.");
            event.preventDefault();
            event.stopPropagation();
            return;
        }

        if (event.key === 'F11' || event.keyCode === 122) {
            console.log("Chặn F11");
            event.preventDefault();
            event.stopPropagation();
            return;
        }

        if (event.altKey && (event.key === 'Tab' || event.keyCode === 9)) {
            console.log("Phát hiện Alt+Tab → cộng lần vi phạm.");
            violationCount++;
            if (violationCount > maxViolations) {
                console.log("Vượt quá số lần vi phạm cho phép → kick khỏi bài thi.");
                callResetTimeAndRedirect();
            } else {
                showWarningModal(`⚠️ Phát hiện bạn dùng Alt+Tab.<br>Lần vi phạm: ${violationCount}/${maxViolations}.<br>Nếu vượt quá giới hạn, bài thi sẽ kết thúc.`);
            }
            event.preventDefault();
            event.stopPropagation();
            return;
        }

        if (event.altKey && (event.key === 'F4' || event.keyCode === 115)) {
            console.log("Chặn Alt+F4");
            event.preventDefault();
            event.stopPropagation();
            return;
        }

        if (event.ctrlKey && event.altKey && (event.key === 'Delete' || event.keyCode === 46)) {
            console.log("Chặn Ctrl+Alt+Delete");
            event.preventDefault();
            event.stopPropagation();
            return;
        }

    }, true);
};

function callResetTimeAndRedirect() {
    if (window.dotnetHelper) {
        window.dotnetHelper.invokeMethodAsync('ResetExamTime')
            .then(() => {
                console.log("Đã gọi ResetExamTime bên Blazor");
            })
            .catch(err => {
                console.error("Lỗi gọi ResetExamTime:", err);
            });
    } else {
        console.log("Chưa có dotnetHelper. Điều hướng luôn.");
    }
}

function showFullscreenModal(goFullscreen) {
    const existing = document.getElementById("fullscreen-modal");
    if (existing) existing.remove();

    const modal = document.createElement("div");
    modal.id = "fullscreen-modal";
    modal.style.position = "fixed";
    modal.style.top = 0;
    modal.style.left = 0;
    modal.style.width = "100%";
    modal.style.height = "100%";
    modal.style.background = "rgba(0,0,0,0.8)";
    modal.style.display = "flex";
    modal.style.justifyContent = "center";
    modal.style.alignItems = "center";
    modal.style.zIndex = 9999;

    const button = document.createElement("button");
    button.innerText = "Vào lại Fullscreen";
    button.style.padding = "20px 40px";
    button.style.fontSize = "18px";
    button.style.cursor = "pointer";
    button.style.border = "none";
    button.style.borderRadius = "8px";
    button.style.backgroundColor = "#007bff";
    button.style.color = "#fff";
    button.style.boxShadow = "0 4px 10px rgba(0,0,0,0.3)";

    button.onclick = () => {
        goFullscreen();
        modal.remove();
    };

    modal.appendChild(button);
    document.body.appendChild(modal);
}

function showWarningModal(message) {
    const existing = document.getElementById("warning-modal");
    if (existing) existing.remove();

    const modal = document.createElement("div");
    modal.id = "warning-modal";
    modal.style.position = "fixed";
    modal.style.top = 0;
    modal.style.left = 0;
    modal.style.width = "100%";
    modal.style.height = "100%";
    modal.style.background = "rgba(0,0,0,0.7)";
    modal.style.display = "flex";
    modal.style.justifyContent = "center";
    modal.style.alignItems = "center";
    modal.style.zIndex = 9999;

    const box = document.createElement("div");
    box.style.background = "white";
    box.style.padding = "30px";
    box.style.borderRadius = "8px";
    box.style.fontSize = "18px";
    box.style.color = "#333";
    box.style.maxWidth = "400px";
    box.style.textAlign = "center";
    box.innerHTML = `<p>${message}</p>`;

    const button = document.createElement("button");
    button.innerText = "Tôi hiểu";
    button.style.marginTop = "20px";
    button.style.padding = "10px 20px";
    button.style.fontSize = "16px";
    button.style.cursor = "pointer";
    button.onclick = () => {
        modal.remove();
    };

    box.appendChild(button);
    modal.appendChild(box);
    document.body.appendChild(modal);
}
document.addEventListener('DOMContentLoaded', () => {
    const intervalTime = 5000;
    let intervalId;
    let hasCheatingHandled = false; // Chỉ báo 1 lần

    const fetchData = async () => {
        try {
            if (!window.dotnetHelper) {
                console.warn("⚠️ dotnetHelper chưa khởi tạo.");
                return;
            }

            const result = await window.dotnetHelper.invokeMethodAsync("load");
            console.log("📥 Kết quả load:", result);

            // Nếu gian lận và chưa xử lý lần nào → xử lý và ngừng gọi
            if (result === true && !hasCheatingHandled) {
                hasCheatingHandled = true;  // đánh dấu đã xử lý
                clearInterval(intervalId);  // dừng vòng lặp
            }
        } catch (error) {
            console.error("❌ Lỗi khi gọi load():", error);
        }
    };

    fetchData(); // Gọi lần đầu
    intervalId = setInterval(fetchData, intervalTime); // Gọi lặp lại
});


