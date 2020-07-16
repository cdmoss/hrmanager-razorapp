﻿
ToggleCertificateExpiries('foodsafe-check', 'foodsafe-date');
ToggleCertificateExpiries('cpr-check', 'cpr-date');
ToggleCertificateExpiries('firstaid-check', 'firstaid-date');
initializeWorkExperiences();

function initializeWorkExperiences() {
    let checkboxes = document.querySelectorAll('*[id^="workexp-current"]');
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked === true) {
            let checkedId = checkboxes[i].id[checkboxes[i].id.length - 1];
            let workExperienceEndDateId = "workexp-current-enddate-" + checkedId;
            let workExperienceLabelId = "workexp-current-label-" + checkedId;
            let workExperienceEndDate = document.getElementById(workExperienceEndDateId);
            let workExperienceLabel = document.getElementById(workExperienceLabelId);
            workExperienceEndDate.style.display = 'none';
            workExperienceLabel.style.display = 'none';
        }
    }
}

function ToggleCertificateExpiries(checkboxId, dateId) {
    if ($(`#${checkboxId}`).prop("checked") == true) {
        $(`#${dateId}`).prop('disabled', false);
    }
    else if ($(`#${checkboxId}`).prop("checked") == false) {
        $(`#${dateId}`).prop('disabled', true);
    }
}

function makeWorkExperienceCurrent(workExpId) {

    // get the ids of the selected work experience elements
    let workExperienceCheckId = "workexp-current-check-" + workExpId;
    let workExperienceEndDateId = "workexp-current-enddate-" + workExpId;
    let workExperienceLabelId = "workexp-current-label-" + workExpId;

    // get the elements using the ids
    let workExperienceCheck = document.getElementById(workExperienceCheckId);
    let workExperienceEndDate = document.getElementById(workExperienceEndDateId);
    let workExperienceLabel = document.getElementById(workExperienceLabelId);

    // get all checkboxes that weren't selected
    let checkboxes = $('[id^="workexp-current"]');

    // hide/show end date and other checkboxes based on checked value
    if (workExperienceCheck.checked === true) {
        workExperienceEndDate.style.display = 'none';
        workExperienceLabel.style.display = 'none';
        workExperienceEndDate.valueAsDate = new Date(1970, 1, 1);
    }
    else {
        workExperienceLabel.style.display = 'block';
        workExperienceEndDate.style.display = 'block';
    }
}

//function check() {
//    if ($("#Volunteer_FirstAid").prop("checked") == true) {
//        $('#date').prop('disabled', false);
//    }
//    else if ($("#Volunteer_FirstAid").prop("checked") == false) {
//        $('#date').prop('disabled', true);
//        $('#date').prop('value', '');
//    }
//}

//function check2() {
//    if ($("#Volunteer_FoodSafe").prop("checked") == true) {
//        $('#date2').prop('disabled', false);
//    }
//    else if ($("#Volunteer_FoodSafe").prop("checked") == false) {
//        $('#date2').prop('disabled', true);
//        $('#date2').prop('value', '');
//    }
//}

//function check3() {
//    if ($("#Volunteer_Cpr").prop("checked") == true) {
//        $('#date3').prop('disabled', false);
//    }
//    else if ($("#Volunteer_Cpr").prop("checked") == false) {
//        $('#date3').prop('disabled', true);
//        $('#date3').prop('value', '');
//    }
//}