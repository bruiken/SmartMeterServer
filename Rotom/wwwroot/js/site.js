$(() => {
    const toastElList = document.querySelectorAll('.toast');
    const toastList = [...toastElList].map(toastEl => new bootstrap.Toast(toastEl, {}));
    toastList.forEach((e) => e.show());
});
