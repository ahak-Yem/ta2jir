function showConfirmationBox() {
	var deleteBtn = document.getElementById('delete-btn');
	deleteBtn.addEventListener('click', showConfirmationBox);
	var confirmationBox = document.querySelector('.confirmation-box');
	var overlay = document.createElement('div');
	overlay.classList.add('overlay');
	document.body.appendChild(overlay);
	setTimeout(function () {
		confirmationBox.classList.add('show');
		overlay.classList.add('show');
	}, 150);
}

function hideConfirmationBox() {
	var confirmationBox = document.querySelector('.confirmation-box');
	var overlay = document.querySelector('.overlay');
	confirmationBox.classList.remove('show');
	overlay.classList.remove('show');
	setTimeout(function () {
		overlay.remove();
	}, 300);
}