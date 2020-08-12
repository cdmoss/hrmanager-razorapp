function updateDtps() {
    $('.datetimepicker-time').datetimepicker(
    {
        format: 'HH:mm'
    });

    $('.datetimepicker-datetime').datetimepicker(
    {
        format: 'YYYY-MM-DD HH:mm',
    });
};