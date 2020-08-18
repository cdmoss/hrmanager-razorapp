
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

    $('.datetimepicker-date').datetimepicker(
    {
        date: moment(),
        format: 'YYYY-MM-DD'
    });

    $('.datetimepicker-datetime-withdate').datetimepicker(
    {
        date: moment(),
        format: 'YYYY-MM-DD HH:mm'
        });

    $('.datetimepicker-datetime-link-start').datetimepicker({
        maxDate: moment().add(1, 'days').startOf('day'),
        format: 'YYYY-MM-DD HH:mm'
    });

    $('.datetimepicker-datetime-link-end').datetimepicker({
        minDate: moment(),
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

    $(".datetimepicker-datetime-link-start").on("change.datetimepicker", function (e) {
        $('.datetimepicker-datetime-link-end').datetimepicker('minDate', e.date);
    });

    $(".datetimepicker-datetime-link-end").on("change.datetimepicker", function (e) {
        $('.datetimepicker-datetime-link-start').datetimepicker('maxDate', e.date);
    });

    $(".datetimepicker-date-link-start").on("change.datetimepicker", function (e) {
        $('.datetimepicker-date-link-end').datetimepicker('minDate', e.date);
    });

    $(".datetimepicker-date-link-end").on("change.datetimepicker", function (e) {
        $('.datetimepicker-date-link-start').datetimepicker('maxDate', e.date);
    });
};

updateDtps();