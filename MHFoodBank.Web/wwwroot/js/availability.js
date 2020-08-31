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

    const removeButtonIdIndex = Number(button.id.charAt(button.id.length - 1));
    const wrapper = Array.from($("." + day + "-field"));

    let fieldsAfterRemoveField = wrapper.slice(removeButtonIdIndex);

    fieldsAfterRemoveField.forEach(function (field){
        const initialIdString = field.getElementsByClassName('input-group')[0].id;
        const newFieldIndex = Number(initialIdString.charAt(initialIdString.length - 1)) - 1;
        const dtpDiv = Array.from(field.getElementsByClassName('input-group'));
        const dtpRemove = field.getElementsByTagName('BUTTON')[0];

        dtpRemove.id = "remove-" + day + "-" + newFieldIndex;

        dtpDiv.forEach(function (div){
            div.id = div.id.slice(0, div.id.length - 1) + newFieldIndex;
            const input = div.getElementsByTagName('INPUT')[0];
            const dtpButton = div.getElementsByClassName('input-group-append')[0];
            input.name = input.name.slice(0, input.name.length - 1) + newFieldIndex;
            input.setAttribute('data-target', '#' + div.id);
            dtpButton.setAttribute('data-target', '#' + div.id);
        });

    });

    button.parentElement.remove();
};

function addField(day) {
        const wrapper = $("." + day + "-fields");
        let removeButton;
        if (fieldCount[day] < 5) { //max input box allowed
            fieldCount[day]++;
            removeButton = "remove-" + day + "-" + fieldCount[day];
            $(wrapper).append("<div class='row ml-1 " + day + "-field'><div class='form-group'><div class='input-group date datetimepicker-time-start' id='dtp-" + day + "-1-" + fieldCount[day] + "' data-target-input='nearest'><input name='" + day + "-1-" + fieldCount[day] + "' type='text' class='form-control datetimepicker-input' data-target='#dtp-" + day + "-1-" + fieldCount[day] + "' /><div class='input-group-append' data-target='#dtp-" + day + "-1-" + fieldCount[day] + "' data-toggle='datetimepicker'><div class='input-group-text'><i class='far fa-clock'></i></div></div></div></div ><span class='col-md-1 justify-content-center'>to</span><div class='form-group'><div class='input-group date datetimepicker-time-end' id='dtp-" + day + "-2-" + fieldCount[day] + "' data-target-input='nearest'><input name='" + day + "-2-" + fieldCount[day] + "' type='text' class='form-control datetimepicker-input' data-target='#dtp-" + day + "-2-" + fieldCount[day] + "' /><div class='input-group-append' data-target='#dtp-" + day + "-2-" + fieldCount[day] + "' data-toggle='datetimepicker'><div class='input-group-text'><i class='far fa-clock'></i></div></div></div></div><button type='button' id='" + removeButton + "' class='btn btn-light ml-2 form-group'><i class='fas fa-trash'></i></button></div>"); //add input box

            $(".datetimepicker-time-start").datetimepicker({
                format: "HH:mm"
            });

            $(".datetimepicker-time-end").datetimepicker({
                format: "HH:mm"
            });

            $(".datetimepicker-time-start").on("change.datetimepicker", function (e) {
                const endTimeDiv = this.parentElement.parentElement.getElementsByClassName('datetimepicker-time-end')[0];
                $(endTimeDiv).datetimepicker('minDate', moment({ h: e.date.hour(), m: e.date.minutes() + 1 }));
            });

            $(".datetimepicker-time-end").on("change.datetimepicker", function (e) {
                const startTimeDiv = this.parentElement.parentElement.getElementsByClassName('datetimepicker-time-start')[0]
                $(startTimeDiv).datetimepicker('maxDate', moment({ h: e.date.hour(), m: e.date.minutes() - 1 }));
            });

            let newRemove = document.getElementById(removeButton);
            newRemove.onclick = removeField.bind(this, newRemove, day);
        }

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