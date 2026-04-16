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
