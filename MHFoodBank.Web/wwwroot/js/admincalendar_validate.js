jQuery.validator.addMethod('compareDates', function (value, element, params) {
    var field_1 = $('#' + params[0]).val(),
        field_2 = $('#' + params[1]).val();
    var dateCheck = field_1 > field_2;
    return dateCheck;
}, "The selected dates are not valid");