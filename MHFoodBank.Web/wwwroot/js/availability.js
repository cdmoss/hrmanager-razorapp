// time field adders
$(".time").timepicker({
    'timeFormat': 'G:i',
    'step': 15,
    'minTime': '6:00',
    'maxTime': '22:00'
});
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
            $(wrapper).append("<div class='row form-group ml-1'><input type='text' name='" + day + "-1-" + fieldCount[day] + "' class='time start-time form-control col-md-2' /><span class='col-md-1 justify-content-center'>to</span><input type='text' name='" + day + "-2-" + fieldCount[day] + "' class='time end-time form-control col-md-2' /><input type='button' value='Remove' class='remove_field btn-sm btn btn-light col-md-2 ml-2'/></div>"); //add input box
            $(".time").timepicker({
                'timeFormat': 'G:i',
                'step': 15,
                'minTime': '6:00',
                'maxTime': '22:00'
            });
        }

        $(wrapper).on("click",
            ".remove_field",
            function () { //user click on remove text
                e.preventDefault();
                fieldCount[day]--;
                $(this).parent('div').remove();
            });
    });

    $(".remove_field").click(function () { //user click on remove text
        fieldCount[day]--;
        $(this).parent('div').remove();
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