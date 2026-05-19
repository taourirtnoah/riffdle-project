(function () {
	function formatDate(value) {
		if (!(value instanceof Date) || Number.isNaN(value.getTime())) {
			return "";
		}

		return new Intl.DateTimeFormat(undefined, {
			year: "numeric",
			month: "2-digit",
			day: "2-digit",
			hour: "2-digit",
			minute: "2-digit"
		}).format(value);
	}

	function parseDateTime(text) {
		const value = text.trim();
		if (!value) {
			return null;
		}

		const isoCandidate = new Date(value);
		if (!Number.isNaN(isoCandidate.getTime())) {
			return isoCandidate;
		}

		const hrMatch = value.match(/^(\d{1,2})\.(\d{1,2})\.(\d{4})\s+(\d{1,2}):(\d{2})$/);
		if (hrMatch) {
			const [, day, month, year, hour, minute] = hrMatch;
			return new Date(Number(year), Number(month) - 1, Number(day), Number(hour), Number(minute));
		}

		const usMatch = value.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})\s+(\d{1,2}):(\d{2})(?:\s*([AP]M))?$/i);
		if (usMatch) {
			const [, month, day, year, hourPart, minute, meridiem] = usMatch;
			let hour = Number(hourPart);
			if (meridiem) {
				const upper = meridiem.toUpperCase();
				if (upper === "PM" && hour < 12) {
					hour += 12;
				}
				if (upper === "AM" && hour === 12) {
					hour = 0;
				}
			}
			return new Date(Number(year), Number(month) - 1, Number(day), hour, Number(minute));
		}

		return null;
	}

	document.addEventListener("DOMContentLoaded", () => {
		document.querySelectorAll("[data-datetime-control]").forEach((control) => {
			const input = control.querySelector("input[type='text']");
			if (!input) {
				return;
			}

			input.addEventListener("blur", () => {
				const parsed = parseDateTime(input.value);
				if (parsed) {
					input.value = formatDate(parsed);
				}
			});
		});
	});
})();
