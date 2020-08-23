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
    button.parentElement.remove();
};

function addField(button, day) {
        const wrapper = $("." + day + "-fields");
        let removeButton;
        if (fieldCount[day] < 5) { //max input box allowed
            fieldCount[day]++;
            removeButton = "remove-" + day + "-" + fieldCount[day];
            $(wrapper).append("<div class='row ml-1 " + day + "-field'><div class='form-group'><div class='input-group date datetimepicker-time' id='dtp-" + day + "-1-" + fieldCount[day] + "' data-target-input='nearest'><input name='" + day + "-1-" + fieldCount[day] + "' type='text' class='form-control datetimepicker-input' data-target='#dtp-" + day + "-1-" + fieldCount[day] + "' /><div class='input-group-append' data-target='#dtp-" + day + "-1-" + fieldCount[day] + "' data-toggle='datetimepicker'><div class='input-group-text'><i class='far fa-clock'></i></div></div></div></div ><span class='col-md-1 justify-content-center'>to</span><div class='form-group'><div class='input-group date datetimepicker-time' id='dtp-" + day + "-2-" + fieldCount[day] + "' data-target-input='nearest'><input name='" + day + "-2-" + fieldCount[day] + "' type='text' class='form-control datetimepicker-input' data-target='#dtp-" + day + "-2-" + fieldCount[day] + "' /><div class='input-group-append' data-target='#dtp-" + day + "-2-" + fieldCount[day] + "' data-toggle='datetimepicker'><div class='input-group-text'><i class='far fa-clock'></i></div></div></div></div><button type='button' id='" + removeButton + "' class='btn btn-light ml-2 form-group'><i class='fas fa-trash'></i></button></div>"); //add input box

            $(".datetimepicker-time-start").datetimepicker({
                format: "HH:mm"
            });

            $(".datetimepicker-time-end").datetimepicker({
                format: "HH:mm"
            });
        }

        let newRemove = document.getElementById(removeButton);
        newRemove.onclick = removeField.bind(this, newRemove, day);
}

// availability validators
$.validator.addMethod("endtime_greater_starttime",
    function (value, element) {
        return Date.parse($(".end-time").val()) > Date.parse($(".start-time").val());
    },
    "End time must be after start time.");

$("#availability").validate();
$(".time").rules("add",
    {
        endtime_greater_starttime: true
    });