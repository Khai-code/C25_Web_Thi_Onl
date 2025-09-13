
    // Gán thời gian hiện tại cho ô "Create_Time"
    window.onload = function () {
      const now = new Date();
    const localTime = new Date(now.getTime() - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
    document.getElementById('createTime').value = localTime;
    };

window.showAlert = function (title, message, icon) {
    return Swal.fire({
        title: title,
        text: message,
        icon: icon,
        confirmButtonText: 'Yes',
        showCancelButton: false, // ❌ ẩn nút No
        allowOutsideClick: false, // không cho click ra ngoài
        allowEscapeKey: false     // không cho ấn ESC để thoát
    }).then((result) => {
        return result.isConfirmed; // chỉ có Yes => true
    });
};
