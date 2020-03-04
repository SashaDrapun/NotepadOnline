const mainCheckBox = document.querySelector('#mainCheckBox');
const allCheckBoxes = document.querySelectorAll('input[type=checkBox]:not(#mainCheckBox)');

const OnChangeMainCheckBox = (event) => {
    const checked = event.target.checked;

    for (let i = 0; i < allCheckBoxes.length; i++) {
        allCheckBoxes[i].checked = checked;
        const valueAttribute = checked ? 'checkedId' : '';
        allCheckBoxes[i].setAttribute('name', valueAttribute);
    }
}

const OnChangeAllCheckBoxes = (event) => {
    const target = event.target;
    const checked = event.target.checked;

    if (checked) {
        target.setAttribute('name', 'checkedId');
    }
    else {
        target.setAttribute('name', '');
    }
}

mainCheckBox.addEventListener('change', OnChangeMainCheckBox);

for (let i = 0; i < allCheckBoxes.length; i++) {
    allCheckBoxes[i].addEventListener('change', OnChangeAllCheckBoxes);
}