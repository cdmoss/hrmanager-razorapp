// Get the input field
let input = document.getElementsByClassName("pass");

// Get the warning text
let text = document.getElementsByClassName("text");

for (i = 0; i < input.length; i++) {
    // When the user presses any key on the keyboard, run the function
    input[i].addEventListener("keyup", function (event) {
        checkCapsLock();
    });
}

function checkCapsLock() {
    // If "caps lock" is pressed, display the warning text
    Array.from(text).forEach(function (input) {
        if (event.getModifierState("CapsLock")) {
            input.style.display = "inline";
        } else {
            input.style.display = "none";
        }
    });
}