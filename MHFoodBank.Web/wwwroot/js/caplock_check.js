// Get the input field
let input = document.getElementsByClassName("pass");

// Get the warning text
let text = document.getElementById("text");

for (i = 0; i < input.length; i++) {
    // When the user presses any key on the keyboard, run the function
    input[i].addEventListener("keyup", function (event) {
        checkCapsLock();
    });
}

function checkCapsLock() {
    // If "caps lock" is pressed, display the warning text
    if (event.getModifierState("CapsLock")) {
        text.style.display = "inline";
    } else {
        text.style.display = "none";
    }
}