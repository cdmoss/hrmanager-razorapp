// time field adders
var fieldCount = {
    "monday": 1,
    "tuesday": 1,
    "wednesday": 1,
    "thursday": 1,
    "friday": 1,
    "saturday": 1,
    "sunday": 1
}

var timeButtons = $(".add-field");
Array.prototype.forEach.call(timeButtons, function (button) {
    var day = button.id;
    $(button).click(function (e) {
        var wrapper = $("." + day + "-fields");
        e.preventDefault();
        if (fieldCount[day] < 5) { //max input box allowed
            fieldCount[day]++;
            $(wrapper).append("<div class='row ml-1'><div class='form-group'><div class='input-group date datetimepicker-time' id='dtp-" + day + "-1-" + fieldCount[day] + "' data-target-input='nearest'><input name='" + day + "-1-" + fieldCount[day] + "' type='text' class='form-control datetimepicker-input' data-target='#dtp-" + day + "-1-" + fieldCount[day] + "' /><div class='input-group-append' data-target='#dtp-" + day + "-1-" + fieldCount[day] + "' data-toggle='datetimepicker'><div class='input-group-text'><i class='far fa-clock'></i></div></div></div></div ><span class='col-md-1 justify-content-center'>to</span><div class='form-group'><div class='input-group date datetimepicker-time' id='dtp-" + day + "-2-" + fieldCount[day] + "' data-target-input='nearest'><input name='" + day + "-2-" + fieldCount[day] + "' type='text' class='form-control datetimepicker-input' data-target='#dtp-" + day + "-2-" + fieldCount[day] + "' /><div class='input-group-append' data-target='#dtp-" + day + "-2-" + fieldCount[day] + "' data-toggle='datetimepicker'><div class='input-group-text'><i class='far fa-clock'></i></div></div></div></div><button class='remove_field btn btn-light ml-2 form-group'><i class='fas fa-trash'></i></button></div>"); //add input box
        }

        $(wrapper).on("click",
            ".remove_field",
            function () { //user click on remove text
                e.preventDefault();
                fieldCount[day]--;
                $(this).parent('div').remove();
            });
    });
});

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