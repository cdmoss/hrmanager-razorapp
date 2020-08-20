function populateSingleShiftModal(selectedShift) {

    // determine if shift is open
    let shiftIsOpen = selectedShift.extendedProps.vol === "Open";

    let selectedShiftWasPartOfRecurringSet = selectedShift.extendedProps.wasPartOfRecurring !== "0";

    selectedShiftInitialStartDate = new Date(selectedShift.start);
    selectedShiftEndDate = new Date(selectedShift.end);

    let startTime = moment(selectedShiftInitialStartDate).format('HH:mm');
    let endTime = moment(selectedShiftEndDate).format('HH:mm');

    document.getElementById("edit-shift-id").value = selectedShift.id;
    document.getElementById("edit-shift-date").value = moment(selectedShiftInitialStartDate).format('yyyy-MM-DD');
    document.getElementById("edit-shift-position").value = (selectedShift.extendedProps.posWorked);

    // validation must be initialized before values are assigned to inputs to prevent starttime from being assigned to both, thanks tempus
    $('#dtp-shift-edit-endtime').datetimepicker('minDate', startTime);
    $('#dtp-shift-edit-starttime').datetimepicker('maxDate', endTime);

    $("dtp-shift-edit-starttime").on("change.datetimepicker", function (e) {
        const endTimeInput = document.getElementsById('edit-shift-endtime');
        let originalDate = moment(endTimeInput.value, 'HH:mm');
        $('dtp-shift-edit-endtime').datetimepicker('minDate', moment({ h: e.date.hour(), m: e.date.minutes() + 1 }));
        endTimeInput.value = originalDate.format('HH:mm');
    });

    $("#dtp-shift-edit-endtime").on("change.datetimepicker", function (e) {
        const startTimeInput = startTimeDiv.getElementsById('edit-shift-starttime')[0];
        let originalDate = moment(startTimeInput.value, 'HH:mm');
        $('dtp-shift-edit-starttime').datetimepicker('maxDate', moment({ h: e.date.hour(), m: e.date.minutes() - 1 }));
        startTimeInput.value = originalDate.format('HH:mm');
    });

    document.getElementById("edit-shift-starttime").value = startTime;
    document.getElementById("edit-shift-endtime").value = endTime;

    let volunteerFullNameWithId = selectedShift.extendedProps.vol;

    if (shiftIsOpen) {
        document.getElementById("open-shift-edit").checked = true;
        setShiftAsOpenEdit();
    }

    else {
        document.getElementById("open-shift-edit").checked = false;
        document.getElementById("edit-shift-volunteer").value = volunteerFullNameWithId;
    }
    
    if (selectedShiftWasPartOfRecurringSet) {
        $('#edit-shift-delete-prompt').prop('style', 'display: block');
        $('#edit-shift-delete-noprompt').prop('style', 'display: none');
        document.getElementById('edit-recshift-single-confirm-id').value = selectedShift.id
    }
    else {
        $('#edit-shift-delete-prompt').prop('style', 'display: none');
        $('#edit-shift-delete-noprompt').prop('style', 'display: block');
    }
}

function autoFillShiftModal() {
    var currentDate = new Date();
    document.getElementById("add-shift-date").value = moment(currentDate).format('yyyy-MM-DD');
    document.getElementById("add-shift-starttime").value = "8:30";
    document.getElementById("add-shift-endtime").value = "16:30";

    $('#add-shift-modal').modal();
}