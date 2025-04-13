
    // Gán thời gian hiện tại cho ô "Create_Time"
    window.onload = function () {
      const now = new Date();
    const localTime = new Date(now.getTime() - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
    document.getElementById('createTime').value = localTime;
    };

