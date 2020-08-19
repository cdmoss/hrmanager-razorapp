
function updateDtps() {
    $('.datetimepicker-time-start').datetimepicker(
    {
        format: 'HH:mm',
    });

    $('.datetimepicker-time-end').datetimepicker(
    {
        format: 'HH:mm'
    });

    $('.datetimepicker-datetime').datetimepicker(
    {
        format: 'YYYY-MM-DD HH:mm'
    });

    $('.datetimepicker-date-withdate').datetimepicker(
    {
        date: moment(),
        format: 'YYYY-MM-DD'
    });

    $('.datetimepicker-date').datetimepicker(
    {
        format: 'YYYY-MM-DD'
    });

    $('.datetimepicker-datetime-withdate').datetimepicker(
    {
        date: moment(),
        format: 'YYYY-MM-DD HH:mm'
    });

    $('.datetimepicker-datetime-link-start-entry').datetimepicker({
        format: 'YYYY-MM-DD HH:mm'
    });

    $('.datetimepicker-datetime-link-end-entry').datetimepicker({
        format: 'YYYY-MM-DD HH:mm'
    });

    $('.datetimepicker-date-link-start').datetimepicker(
    {
        format: 'YYYY-MM-DD'
    });

    $('.datetimepicker-date-link-end').datetimepicker(
    {
        format: 'YYYY-MM-DD'
    });

    $(".datetimepicker-date-link-start").on("change.datetimepicker", function (e) {
        $('.datetimepicker-date-link-end').datetimepicker('minDate', e.date);
    });

    $(".datetimepicker-date-link-end").on("change.datetimepicker", function (e) {
        $('.datetimepicker-date-link-start').datetimepicker('maxDate', e.date);
    });
};

updateDtps();