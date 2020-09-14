// time field adders
//This gets filled in the ManageAvailibility.cshtml scripts at the bottom of the page
let fieldCount = {
    "monday": document.getElementsByClassName("monday-field").length,
    "tuesday": document.getElementsByClassName("tuesday-field").length,
    "wednesday": document.getElementsByClassName("wednesday-field").length,
    "thursday": document.getElementsByClassName("thursday-field").length,
    "friday": document.getElementsByClassName("friday-field").length,
    "saturday": document.getElementsByClassName("saturday-field").length,
    "sunday": document.getElementsByClassName("sunday-field").length
};

function removeField(button, day) {
    fieldCount[day]--;

    const fieldIndex = Number(button.id.charAt(button.id.length - 1));
    const wrapper = Array.from($("." + day + "-field"));

    let fieldsAfterRemoveField = wrapper.slice(fieldIndex);

    fieldsAfterRemoveField.forEach(function (field){
        const inputs = field.getElementsByTagName('INPUT');
        const inputName = inputs[0].name;
        const newFieldIndex = Number(inputName.charAt(inputName.length - 1)) - 1;

        for (var i = 0; i < 2; i++) {
            inputs[i].name = inputs[i].name.substring(0, inputs[i].name.length - 1) + newFieldIndex
        }

        const removeButton = field.getElementsByTagName('BUTTON')[0];

        removeButton.id = "remove-" + day + "-" + newFieldIndex;
    });

    button.parentElement.remove();
};

function addField(day) {
        const wrapper = $("." + day + "-fields");
        let removeButton;
        if (fieldCount[day] < 5) { //max input box allowed
            fieldCount[day]++;
            removeButton = "remove-" + day + "-" + fieldCount[day];
            $(wrapper).append("<div class='row mt-1 ml-1 form-group " + day + "-field time-field'><input name='" + day + "-1-" + fieldCount[day] + "' type='text' class='col-md-3 form-control time start' /><span class='col-md-1 justify-content-center'>to</span><input name='" + day + "-2-" + fieldCount[day] +"' type='text' class='col-md-3 form-control time end' /><button type='button' id='" + removeButton + "' class='btn btn-light ml-2'><i class='fas fa-trash'></i></button></div >");

            $('.time').timepicker({
                'showDuration': true,
                'timeFormat': 'G:i',
                'show2400': true,
                'maxTime': '23:59',
                'defaultTimeDelta': '60000',
                'step': '15',
                'disableTextInput': true
            });

            // initialize datepair
            $('.time-field').datepair();

            let newRemove = document.getElementById(removeButton);
            newRemove.onclick = removeField.bind(this, newRemove, day);
        }

}