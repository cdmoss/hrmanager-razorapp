// causes fields in volunteer details to be editable on click

$('.editable').click(function () {
    $('.cancel').click();

    originalText = this;
    input = document.createElement("input");
    input.setAttribute("type", "text");
    input.value = originalText.textContent;
    input.classList.add("form-control");
    input.classList.add("col-md-6");
    input.classList.add("being-edited");
    parent = originalText.parentNode;

    // build save button 
    saveButton = document.createElement("button");
    saveButton.classList.add("btn");
    saveButton.classList.add("btn-sm");
    saveButton.classList.add("btn-success");
    saveButton.classList.add("col-md-2");
    saveButton.classList.add("ml-1");
    saveButton.classList.add("mr-1");
    saveButton.classList.add("save");
    saveButton.innerHTML = "<i class='fas fa-check'></i>";

    // build cancel button
    cancelButton = document.createElement("button");
    cancelButton.classList.add("btn");
    cancelButton.classList.add("btn-sm");
    cancelButton.classList.add("btn-danger");
    cancelButton.classList.add("col-md-2");
    cancelButton.classList.add("ml-1");
    cancelButton.classList.add("cancel");
    cancelButton.innerHTML = "<i class='fas fa-window-close'></i>";

    parent.replaceChild(input, originalText);
    parent.insertBefore(cancelButton, input.nextSibling);
    parent.insertBefore(saveButton, input.nextSibling);
    $(document).on("click",
        ".cancel",
        function() {
            $('.being-edited').replaceWith(originalText);
            $(".save").remove();
            $(".cancel").remove();
        });

    $(document).on("click",
        ".save",
        function () {
            originalText.textContent = input.value;
            $('.being-edited').replaceWith(originalText);
            $(".save").remove();
            $(".cancel").remove();
        });
});

function recoverOriginal(original, sender) {
    var newParent = sender.parentElement;
    $(".save").remove(); 
    $(".cancel").remove();
    newParent.replaceChild($('.being-edited'), original);
}

$('.editable').hover(
    function () {
        $(this).css('color', 'blue');
        $(this).css('cursor', 'pointer');
    },
    function () {
        $(this).css('color', 'black');
        $(this).css('cursor', 'default');
    });