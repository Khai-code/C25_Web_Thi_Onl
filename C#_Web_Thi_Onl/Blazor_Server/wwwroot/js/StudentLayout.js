window.addEventListener("scroll", function () {
    let navbar = document.querySelector(".navbar");
    if (window.scrollY > 50) {
        navbar.classList.add("scrolled");
    } else {
        navbar.classList.remove("scrolled");
    }
});
window.updateNavbar = function () {
    let navbar = document.getElementById("navbar");

    if (!navbar) return;

    let currentPath = window.location.pathname;

    if (currentPath === "/StudentLayout") {
        navbar.classList.add("bg-transparent");
        navbar.style.removeProperty("background-color");
        navbar.style.removeProperty("box-shadow");
    } else {
        navbar.classList.remove("bg-transparent");
        navbar.style.backgroundColor = "rgba(241, 245, 236, 0.95)";
        navbar.style.boxShadow = "0px 4px 10px rgba(0, 0, 0, 0.1)";
    }
};
