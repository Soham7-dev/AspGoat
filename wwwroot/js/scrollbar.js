document.addEventListener("DOMContentLoaded", () => {
    const bar = document.querySelector(".sidebar");   
    if (!bar) return;
  
    // restore saved position
    bar.scrollTop = +sessionStorage.getItem("sidebarScroll") || 0;
  
    // remember current position
    window.addEventListener("beforeunload", () => {
      sessionStorage.setItem("sidebarScroll", bar.scrollTop);
    });
  });