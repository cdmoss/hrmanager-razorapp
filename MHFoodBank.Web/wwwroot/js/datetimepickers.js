function updateDtps() {
    $('.datetimepicker-time').datetimepicker(
    {
        format: 'HH:mm'
    });

    $('.datetimepicker-datetime').datetimepicker(
    {
        date: moment(),
        format: 'YYYY-MM-DD HH:mm',
    });
};