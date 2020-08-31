// creates a object filled with strings for all shift properties from selected shift's date info

function populateRecurringShiftModal(selectedShift) {

    // determine if shift is open
    let shiftIsOpen = selectedShift.extendedProps.vol === "Open";

    let recurringShiftStrings = getStringsFromRecurringselectedShift(selectedShift)

    let weekdayBools = getWeekdayCheckboxValues(selectedShift);

    if (shiftIsOpen) {
        document.getElementById("open-shift-edit-rec").checked = true;
        setRecurringShiftAsOpen();
    }
    else {
        document.getElementById("edit-recshift-volunteer").value = selectedShift.extendedProps.vol;
    }

    // initalize validation (must be done before assigning values to inputs to prevent duplicate assignment errors caused by the minDate/maxDate assignment)
    let startTimeStr = recurringShiftStrings.startTime.split(':');
    let startTime = moment({ h: startTimeStr[0], m: startTimeStr[1] })

    let endTimeStr = recurringShiftStrings.endTime.split(':');
    let endTime = moment({ h: endTimeStr[0], m: endTimeStr[1] });

    $('#dtp-recshift-edit-endtime').datetimepicker('minDate', startTime);
    $('#dtp-recshift-edit-starttime').datetimepicker('maxDate', endTime);

    let startDate = recurringShiftStrings.recurringStartDate;
    let endDate = recurringShiftStrings.recurringEndDate;

    $('#dtp-admincalendar-edit-recshift-enddate').datetimepicker('minDate', startDate)
    $('#dtp-admincalendar-edit-recshift-all-date').datetimepicker('maxDate', endDate)

    $("#dtp-admincalendar-edit-recshift-all-date").on("change.datetimepicker", function (e) {
        const endDateInput = document.getElementById('edit-recshift-enddate');
        let originalDate = moment(endDateInput.value);
        $('#dtp-admincalendar-edit-recshift-enddate').datetimepicker('minDate', e.date);
        endDateInput.value = originalDate.format('YYYY-MM-DD');
    });

    $("#dtp-admincalendar-edit-recshift-enddate").on("change.datetimepicker", function (e) {
        const startDateInput = document.getElementById('edit-recshift-all-startdate');
        let originalDate = moment(startDateInput.value);
        $('.datetimepicker-date-start').datetimepicker('maxDate', e.date);
        startDateInput.value = originalDate.format('YYYY-MM-DD');
    });

    $("#dtp-recshift-edit-starttime").on("change.datetimepicker", function (e) {
        const endTimeInput = document.getElementsById('edit-recshift-endtime');
        let originalDate = moment(endTimeInput.value, 'HH:mm');
        $('dtp-recshift-edit-endtime').datetimepicker('minDate', moment({ h: e.date.hour(), m: e.date.minutes() + 1 }));
        endTimeInput.value = originalDate.format('HH:mm');
    });

    $("#dtp-recshift-edit-endtime").on("change.datetimepicker", function (e) {
        const startTimeInput = startTimeDiv.getElementsById('edit-recshift-starttime')[0];
        let originalDate = moment(startTimeInput.value, 'HH:mm');
        $('dtp-recshift-edit-starttime').datetimepicker('maxDate', moment({ h: e.date.hour(), m: e.date.minutes() - 1 }));
        startTimeInput.value = originalDate.format('HH:mm');
    });

    // embeds id of selected shift into modal form
    document.getElementById("edit-recshift-id").value = selectedShift.id;

    // populates both selected shift and recurring shift start date field
    document.getElementById("edit-recshift-single-initial-startdate").value = recurringShiftStrings.selectedInitialStartDate;
    document.getElementById("edit-recshift-single-final-startdate").value = recurringShiftStrings.selectedFinalStartDate;
    document.getElementById("edit-recshift-all-startdate").value = recurringShiftStrings.recurringStartDate;

    // populates fields with properties of clicked event
    document.getElementById("edit-recshift-enddate").value = recurringShiftStrings.recurringEndDate
    document.getElementById("edit-recshift-starttime").value = recurringShiftStrings.startTime
    document.getElementById("edit-recshift-endtime").value = recurringShiftStrings.endTime
    document.getElementById("edit-recshift-position").value = (selectedShift.extendedProps.posWorked);
    
    // loads selected weekdays for chosen shift into checkboxes
    document.getElementById('edit-recshift-weekday-monday').checked = weekdayBools.monday;
    document.getElementById('edit-recshift-weekday-tuesday').checked = weekdayBools.tuesday;
    document.getElementById('edit-recshift-weekday-wednesday').checked = weekdayBools.wednesday;
    document.getElementById('edit-recshift-weekday-thursday').checked = weekdayBools.thursday;
    document.getElementById('edit-recshift-weekday-friday').checked = weekdayBools.friday;
    document.getElementById('edit-recshift-weekday-saturday').checked = weekdayBools.saturday;
    document.getElementById('edit-recshift-weekday-sunday').checked = weekdayBools.sunday;
}

function getWeekdayCheckboxValues(selectedShift) {

    const recurringShiftHasExcludedShifts = typeof selectedShift._def.recurringDef.typeData._rrule !== "undefined";

    if (recurringShiftHasExcludedShifts) {
        const weekdayFcObject = selectedShift._def.recurringDef.typeData._rrule[0].origOptions.byweekday;
        return {
            monday: weekdayFcObject.find(({ weekday }) => weekday === 0),
            tuesday: weekdayFcObject.find(({ weekday }) => weekday === 1),
            wednesday: weekdayFcObject.find(({ weekday }) => weekday === 2),
            thursday: weekdayFcObject.find(({ weekday }) => weekday === 3),
            friday: weekdayFcObject.find(({ weekday }) => weekday === 4),
            saturday: weekdayFcObject.find(({ weekday }) => weekday === 5),
            sunday: weekdayFcObject.find(({ weekday }) => weekday === 6),
            
        }
    }
    else {
        return {
            monday: selectedShift._def.recurringDef.typeData.options.byweekday.indexOf(0) !== -1,
            tuesday: selectedShift._def.recurringDef.typeData.options.byweekday.indexOf(1) !== -1,
            wednesday: selectedShift._def.recurringDef.typeData.options.byweekday.indexOf(2) !== -1,
            thursday: selectedShift._def.recurringDef.typeData.options.byweekday.indexOf(3) !== -1,
            friday: selectedShift._def.recurringDef.typeData.options.byweekday.indexOf(4) !== -1,
            saturday: selectedShift._def.recurringDef.typeData.options.byweekday.indexOf(5) !== -1,
            sunday: selectedShift._def.recurringDef.typeData.options.byweekday.indexOf(6) !== -1
        }
    }
}

function getStringsFromRecurringselectedShift(selectedShift) {
    // check if the shift has excluded dates as this will determine where to get dtstart and until
    const recurringShiftHasExcludedShifts = typeof selectedShift._def.recurringDef.typeData._rrule !== "undefined";

    // both initial and final start dates are needed to properly resolve changes to start time
    let selectedShiftInitialStartDate = new Date(selectedShift.start);
    let selectedShiftFinalStartDate = new Date(selectedShift.start);
    let selectedShiftEndDate = new Date(selectedShift.end);

    let recurringShiftStartDate;
    let recurringShiftEndDate;

    if (recurringShiftHasExcludedShifts) {
        recurringShiftStartDate = new Date(selectedShift._def.recurringDef.typeData._rrule[0].origOptions.dtstart);
        recurringShiftEndDate = new Date(selectedShift._def.recurringDef.typeData._rrule[0].origOptions.until);
    }
    else {
        recurringShiftStartDate = new Date(selectedShift._def.recurringDef.typeData.origOptions.dtstart);
        recurringShiftEndDate = new Date(selectedShift._def.recurringDef.typeData.origOptions.until);
    }

    let selectedInitialStartDateStr = moment(selectedShiftInitialStartDate).format('yyyy-MM-DD');
    let selectedFinalStartDateStr = moment(selectedShiftFinalStartDate).format('yyyy-MM-DD');
    let recurringStartDateStr = moment(recurringShiftStartDate).format('yyyy-MM-DD');
    let selectedEndDateStr = moment(selectedShiftEndDate).format('yyyy-MM-DD');
    let recurringEndDateStr = moment(recurringShiftEndDate).format('yyyy-MM-DD');
    let startTimeStr = moment(selectedShiftInitialStartDate).format('HH:mm');
    let endTimeStr = moment(selectedShiftEndDate).format('HH:mm');

    return {
        selectedInitialStartDate: selectedInitialStartDateStr,
        selectedFinalStartDate: selectedFinalStartDateStr,
        recurringStartDate: recurringStartDateStr,
        selectedEndDate: selectedEndDateStr,
        recurringEndDate: recurringEndDateStr,
        startTime: startTimeStr,
        endTime: endTimeStr
    }
}