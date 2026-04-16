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
