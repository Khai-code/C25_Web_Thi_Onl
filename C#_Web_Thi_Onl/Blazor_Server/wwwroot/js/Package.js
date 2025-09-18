
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
        confirmButtonText: 'Hủy thi',
        cancelButtonText: 'Xác nhận',
        showCancelButton: true,
        allowOutsideClick: false,
        allowEscapeKey: false,
        confirmButtonColor: '#d33',   // 🔴 đỏ cho "Hủy thi"
        cancelButtonColor: '#3085d6'  // 🔵 xanh cho "Xác nhận"
    }).then((result) => {
        return result.isConfirmed; // bấm "Hủy thi" => true
    });
};
window.closeAlert = function () {
    Swal.close();
};

