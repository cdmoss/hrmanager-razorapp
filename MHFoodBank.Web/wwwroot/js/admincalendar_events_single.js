function populateSingleShiftModal(selectedShift) {

    // determine if shift is open
    let shiftIsOpen = selectedShift.extendedProps.vol === "Open";

    let selectedShiftWasPartOfRecurringSet = selectedShift.extendedProps.wasPartOfRecurring !== "0";

    selectedShiftInitialStartDate = new Date(selectedShift.start);
    selectedShiftEndDate = new Date(selectedShift.end);

    document.getElementById("edit-shift-id").value = selectedShift.id;
    document.getElementById("edit-shift-date").value = moment(selectedShiftInitialStartDate).format('yyyy-MM-DD');
    document.getElementById("edit-shift-position").value = (selectedShift.extendedProps.posWorked);

    $('#edit-shift-starttime').timepicker('setTime', new Date(selectedShiftInitialStartDate));
    $('#edit-shift-endtime').timepicker('setTime', new Date(selectedShiftEndDate));

    $('#edit-shift-time-div').datepair('refresh');

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