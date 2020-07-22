jQuery.validator.addMethod('compareDates', function (value, element, params) {
    const startDate = $('#' + params[0]).val(),
        endDate = $('#' + params[1]).val();
    return this.optional(element) || value === startDate < endDate;
}, "The selected dates are not valid");