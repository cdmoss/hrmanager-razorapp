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
        recurringShiftStartDate = new Date(selectedShift._def.recurringDef.typeData.options.dtstart);
        recurringShiftEndDate = new Date(selectedShift._def.recurringDef.typeData.options.until);
    }
    else {
        recurringShiftStartDate = new Date(selectedShift._def.recurringDef.typeData.origOptions.dtstart);
        recurringShiftEndDate = new Date(selectedShift._def.recurringDef.typeData.origOptions.until);
    }

    let selectedInitialStartDateStr = selectedShiftInitialStartDate.getFullYear() + "-" + appendLeadingZeroes(selectedShiftInitialStartDate.getMonth() + 1) + "-" + appendLeadingZeroes(selectedShiftInitialStartDate.getDate());
    let selectedFinalStartDateStr = selectedShiftFinalStartDate.getFullYear() + "-" + appendLeadingZeroes(selectedShiftFinalStartDate.getMonth() + 1) + "-" + appendLeadingZeroes(selectedShiftFinalStartDate.getDate());
    let recurringStartDateStr = recurringShiftStartDate.getFullYear() + "-" + appendLeadingZeroes(recurringShiftStartDate.getMonth() + 1) + "-" + appendLeadingZeroes(recurringShiftStartDate.getDate());
    let selectedEndDateStr = selectedShiftEndDate.getFullYear() + "-" + appendLeadingZeroes(selectedShiftEndDate.getMonth() + 1) + "-" + appendLeadingZeroes(selectedShiftEndDate.getDate());
    let recurringEndDateStr = recurringShiftEndDate.getFullYear() + "-" + appendLeadingZeroes(recurringShiftEndDate.getMonth() + 1) + "-" + appendLeadingZeroes(recurringShiftEndDate.getDate());
    let startTimeStr = appendLeadingZeroes(selectedShiftInitialStartDate.getHours()) + ":" + appendLeadingZeroes(selectedShiftInitialStartDate.getMinutes());
    let endTimeStr = appendLeadingZeroes(selectedShiftEndDate.getHours()) + ":" + appendLeadingZeroes(selectedShiftEndDate.getMinutes());

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