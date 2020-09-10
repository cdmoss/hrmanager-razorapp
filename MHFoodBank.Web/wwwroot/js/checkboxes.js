
initializeWorkExperiences();

function initializeWorkExperiences() {
    let checkboxes = document.querySelectorAll('*[id^="workexp-current"]');
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked === true) {
            let checkedId = checkboxes[i].id[checkboxes[i].id.length - 1];
            let workExperienceEndDateDiv = document.getElementById("workexp-enddate-" + checkedId);
            workExperienceEndDateDiv.style.display = 'none';
        }
    }
}

function toggleCertificates(checkboxId, inputIds, divId) {
    if ($(`#${checkboxId}`).prop("checked") == true) {
        inputIds.forEach(function (inputId) {
            $(`#${inputId}`).prop('disabled', false);
        })

        $(`#${divId}`).removeClass('d-none');
        $(`#${divId}`).addClass('row col-8');
    }
    else if ($(`#${checkboxId}`).prop("checked") == false) {
        const div = $(`#${divId}`);
        div.prop('display', 'none');

        for (let i = 0; i < inputIds.length; i++) {
            $(`#${inputIds[i]}`).prop('disabled', true);
        }

        $(`#${divId}`).removeClass('row col-8');
        $(`#${divId}`).addClass('d-none');
    }
}

function makeWorkExperienceCurrent(workExpId) {
    // get the elements using the ids
    let workExperienceCheck = document.getElementById("workexp-current-check-" + workExpId);
    let workExperienceEndDateDiv = document.getElementById("workexp-enddate-" + workExpId);

    // get all checkboxes that weren't selected
    let checkboxes = $('[id^="workexp-current"]');

    // hide/show end date and other checkboxes based on checked value
    if (workExperienceCheck.checked === true) {
        workExperienceEndDateDiv.style.display = 'none';
        workExperienceEndDateDiv.getElementsByTagName('INPUT')[0].valueAsDate = new Date(1970, 1, 1);
    }
    else {
        workExperienceEndDateDiv.style.display = 'block';
    }
}
