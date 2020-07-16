function populateSingleShiftModal(selectedShift) {

    // determine if shift is open
    let shiftIsOpen = selectedShift.extendedProps.vol === "Open";

    let selectedShiftWasPartOfRecurringSet = selectedShift.extendedProps.wasPartOfRecurring !== "0";

    selectedShiftInitialStartDate = new Date(selectedShift.start);
    selectedShiftEndDate = new Date(selectedShift.end);
    document.getElementById("edit-shift-id").value = selectedShift.id;
    document.getElementById("edit-shift-date").value = selectedShiftInitialStartDate.getFullYear() + "-" + appendLeadingZeroes(selectedShiftInitialStartDate.getMonth() + 1) + "-" + appendLeadingZeroes(selectedShiftInitialStartDate.getDate());
    document.getElementById("edit-shift-starttime").value = appendLeadingZeroes(selectedShiftInitialStartDate.getHours()) + ":" + appendLeadingZeroes(selectedShiftInitialStartDate.getMinutes());
    document.getElementById("edit-shift-endtime").value = appendLeadingZeroes(selectedShiftEndDate.getHours()) + ":" + appendLeadingZeroes(selectedShiftEndDate.getMinutes());
    document.getElementById("edit-shift-position").value = (selectedShift.extendedProps.posWorked);

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