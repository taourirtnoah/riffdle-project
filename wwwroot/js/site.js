// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
	const guessInput = document.getElementById("guess");
	const suggestions = document.getElementById("guess-suggestions");
	const titlesPayload = document.getElementById("available-song-titles");

	if (!guessInput || !suggestions || !titlesPayload) {
		return;
	}

	let allTitles = [];

	try {
		const parsed = JSON.parse(titlesPayload.textContent || "[]");
		if (Array.isArray(parsed)) {
			allTitles = parsed.filter((title) => typeof title === "string");
		}
	} catch {
		allTitles = [];
	}

	const maxSuggestions = 8;

	function closeSuggestions() {
		suggestions.innerHTML = "";
		suggestions.classList.remove("open");
	}

	function applySuggestion(value) {
		guessInput.value = value;
		closeSuggestions();
		guessInput.focus();
	}

	function renderSuggestions(matches) {
		suggestions.innerHTML = "";

		if (matches.length === 0) {
			closeSuggestions();
			return;
		}

		const fragment = document.createDocumentFragment();

		matches.forEach((match) => {
			const item = document.createElement("li");
			item.className = "guess-suggestion-item";
			item.setAttribute("role", "option");
			item.textContent = match;
			item.addEventListener("mousedown", (event) => {
				event.preventDefault();
				applySuggestion(match);
			});
			fragment.appendChild(item);
		});

		suggestions.appendChild(fragment);
		suggestions.classList.add("open");
	}

	guessInput.addEventListener("input", () => {
		const query = guessInput.value.trim().toLowerCase();
		if (!query) {
			closeSuggestions();
			return;
		}

		const matches = allTitles
			.filter((title) => title.toLowerCase().includes(query))
			.slice(0, maxSuggestions);

		renderSuggestions(matches);
	});

	guessInput.addEventListener("keydown", (event) => {
		if (event.key === "Escape") {
			closeSuggestions();
		}
	});

	document.addEventListener("click", (event) => {
		if (!suggestions.contains(event.target) && event.target !== guessInput) {
			closeSuggestions();
		}
	});
})();

(function () {
	const shareBtn = document.getElementById("share-result-btn");

	if (!shareBtn) {
		return;
	}

	const isCompleted = shareBtn.dataset.isCompleted === "true";
	const isCorrect = shareBtn.dataset.isCorrect === "true";
	const attemptCount = parseInt(shareBtn.dataset.attemptCount, 10);

	shareBtn.addEventListener("click", async (event) => {
		event.preventDefault();

		if (!isCompleted) {
			alert("Complete the daily challenge first!");
			return;
		}

		const siteUrl = window.location.origin;
		const resultStatus = isCorrect
			? `✓ Solved in ${attemptCount} guess${attemptCount === 1 ? "" : "es"}`
			: `✗ Failed to solve (${attemptCount}/${5} guesses)`;

		const shareMessage = `🎸 Riffdle Daily Challenge\n${resultStatus}\n\n${siteUrl}`;

		try {
			await navigator.clipboard.writeText(shareMessage);
			
			const originalText = shareBtn.textContent;
			shareBtn.textContent = "✓ Copied to clipboard";
			setTimeout(() => {
				shareBtn.textContent = originalText;
			}, 2000);
		} catch (err) {
			console.error("Failed to copy to clipboard:", err);
			alert("Could not copy to clipboard. Your result:\n\n" + shareMessage);
		}
	});
})();

(function () {
	if (typeof window.jQuery === "undefined" || typeof window.jQuery.validator === "undefined") {
		return;
	}

	window.jQuery.validator.setDefaults({
		onfocusout: function (element) {
			this.element(element);
		},
		onkeyup: function (element) {
			if (window.jQuery(element).is(":checkbox,:radio")) {
				return;
			}

			this.element(element);
		},
		onclick: function (element) {
			this.element(element);
		},
		highlight: function (element) {
			window.jQuery(element).addClass("is-invalid");
		},
		unhighlight: function (element) {
			window.jQuery(element).removeClass("is-invalid");
		}
	});
})();

(function () {
	function revealSequence(elements, className, delayStep) {
		elements.forEach((element, index) => {
			window.setTimeout(() => {
				element.classList.add(className);
			}, index * delayStep);
		});
	}

	document.addEventListener("DOMContentLoaded", () => {
		document.documentElement.classList.add("js-reveal-ready");

		revealSequence(Array.from(document.querySelectorAll(".detail-card, .table-wrap, .form-actions")), "is-revealed", 70);
		revealSequence(Array.from(document.querySelectorAll(".metal-table tbody tr")), "is-row-revealed", 28);
	});
})();

(function () {
	const dropzone = document.getElementById("attachment-dropzone");
	const fileInput = document.getElementById("attachment-file-input");
	const status = document.getElementById("attachment-upload-status");
	const list = document.getElementById("attachment-list");
	const songIdInput = document.getElementById("song-id");

	if (!dropzone || !fileInput || !status || !list || !songIdInput) {
		return;
	}

	const uploadUrl = dropzone.dataset.uploadUrl || "/Attachment/Upload";
	const songId = songIdInput.value;

	function setStatus(message, isError = false) {
		status.textContent = message;
		status.style.color = isError ? "#ff8d8d" : "";
	}

	async function refreshAttachmentList() {
		const response = await fetch(`/Attachment/GetAttachments?songId=${encodeURIComponent(songId)}`);
		if (!response.ok) {
			throw new Error("Failed to load attachments");
		}
		list.innerHTML = await response.text();
	}

	async function uploadFiles(files) {
		const selectedFiles = Array.from(files || []);
		if (selectedFiles.length === 0) {
			return;
		}

		setStatus(`Uploading ${selectedFiles.length} file${selectedFiles.length === 1 ? "" : "s"}...`);

		for (const file of selectedFiles) {
			const formData = new FormData();
			formData.append("songId", songId);
			formData.append("file", file);

			const response = await fetch(uploadUrl, {
				method: "POST",
				body: formData
			});

			if (!response.ok) {
				throw new Error(`Upload failed for ${file.name}`);
			}
		}

		await refreshAttachmentList();
		setStatus("Upload complete.");
		fileInput.value = "";
	}

	dropzone.addEventListener("click", () => fileInput.click());

	dropzone.addEventListener("dragover", (event) => {
		event.preventDefault();
		dropzone.classList.add("is-dragover");
	});

	dropzone.addEventListener("dragleave", () => {
		dropzone.classList.remove("is-dragover");
	});

	dropzone.addEventListener("drop", async (event) => {
		event.preventDefault();
		dropzone.classList.remove("is-dragover");

		try {
			await uploadFiles(event.dataTransfer.files);
		} catch (error) {
			console.error(error);
			setStatus(error.message || "Upload failed.", true);
		}
	});

	fileInput.addEventListener("change", async () => {
		try {
			await uploadFiles(fileInput.files);
		} catch (error) {
			console.error(error);
			setStatus(error.message || "Upload failed.", true);
		}
	});

	document.addEventListener("DOMContentLoaded", async () => {
		try {
			await refreshAttachmentList();
		} catch (error) {
			console.error(error);
			setStatus("Could not load attachments.", true);
		}
	});
})();
