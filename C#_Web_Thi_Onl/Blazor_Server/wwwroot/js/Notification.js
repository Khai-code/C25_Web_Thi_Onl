window.showToast = (message, type) => {
    let toastContainer = document.getElementById("toastContainer");

    if (!toastContainer) {
        toastContainer = document.createElement("div");
        toastContainer.id = "toastContainer";
        toastContainer.className = "position-fixed top-5 start-50 translate-middle-x p-1";
        toastContainer.style.zIndex = "1050";
        document.body.appendChild(toastContainer);
    }
    let iconHtml = "";
    let textColor = "";
    switch (type) {
        case "success":
            iconHtml = `<i class="bi bi-check-circle-fill text-success fs-6 me-2"></i>`;
            textColor = "text-success";
            break;
        case "error":
            iconHtml = `<i class="bi bi-x-circle-fill text-danger fs-6 me-2"></i>`;
            textColor = "text-danger";
            break;
        case "warning":
            iconHtml = `<i class="bi bi-exclamation-triangle-fill text-warning fs-6 me-2"></i>`;
            textColor = "text-warning";
            break;
        case "info":
            iconHtml = `<i class="bi bi-info-circle-fill text-primary fs-6 me-2"></i>`;
            textColor = "text-primary";
            break;
        default:
            iconHtml = "";
            textColor = "text-dark";
    }
    let toast = document.createElement("div");
    toast.className = `toast align-items-center bg-body-tertiary border-0 show shadow-lg`;
    toast.role = "alert";
    toast.style.padding = "4px 10px";
    toast.style.wordBreak = "break-word";
    toast.style.width = "auto"; 
    toast.style.display = "flex";
    toast.style.alignItems = "center";
    toast.style.marginBottom = "8px";
    toast.style.borderRadius = "8px"; 
    toast.style.border = "1px solid #ddd"; 
    toast.innerHTML = `
        <div class="d-flex align-items-center w-100">
            ${iconHtml}
            <div class="toast-body ${textColor}">${message}</div>
        </div>
    `;
    toastContainer.appendChild(toast);
    setTimeout(() => {
        toast.remove();
    }, 3000);
};
window.showSweetAlert = (message, type) => {
    Swal.fire({
        title: message,
        icon: type,
        showConfirmButton: false, 
        timer: 1500, 
        toast: true, 
        position: "center", 
        customClass: {
            popup: "rounded-lg shadow-lg"
        }
    });
};
