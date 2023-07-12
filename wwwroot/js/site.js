window.onload = function () {
    var inputElement = document.getElementById('myInput');
    var textLength = inputElement.value.length;
    inputElement.setSelectionRange(textLength, textLength);
};
function submitForm() {
    document.getElementById('myForm').submit();
};
