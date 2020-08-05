function updateDtps() {
    $('.datetimepicker-time').datetimepicker(
        {
            format: 'HH:mm'
        });

    $('.datetimepicker-datetime').datetimepicker();

    $('.datetimepicker-date').datetimepicker({
        format: 'L'
    });
};