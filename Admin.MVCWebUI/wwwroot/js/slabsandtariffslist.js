document.addEventListener("DOMContentLoaded", function () {
    const urls = JSON.parse(document.getElementById("Permissions").dataset.urls);

    // Default to LT slabs
    fetchSlabList("LT", urls.getSlabListUrl);

    // Optional: hook up radio buttons
    document.querySelectorAll('input[name="tariffType"]').forEach(radio => {
        radio.addEventListener('change', function () {
            fetchSlabList(this.value, urls.getSlabListUrl);
        });
    });
});

function fetchSlabList(lineType, apiUrl) {
    const fullUrl = `${apiUrl}?listType=${lineType}`;
    const targetAccordion = lineType === "LT" ? "lt-accordion" : "ht-accordion";

    // Clear both accordions
    document.getElementById("lt-accordion").innerHTML = "";
    document.getElementById("ht-accordion").innerHTML = "";

    // Show loading message
    const loadingDiv = document.createElement("div");
    document.getElementById(targetAccordion).appendChild(loadingDiv);

    fetch(fullUrl)
        .then(response => response.json())
        .then(data => {
            // Remove loading message
            loadingDiv.remove();

            if (data.success) {
                renderSlabs(data.data, targetAccordion);
            } else {
                alert("Failed to load slabs");
            }
        })
        .catch(err => {
            console.error("Error fetching slabs:", err);
        });
}

function renderSlabs(slabs, accordionId) {
    const container = document.getElementById(accordionId);

    if (!slabs || slabs.length === 0) {
        container.innerHTML = `<div class="mt-3 text-muted">No ${accordionId.startsWith('lt') ? 'LT' : 'HT'} slabs available.</div>`;
        return;
    }

    slabs.forEach(item => {
        const html = `
            <div class="accordion-item">
                <h2 class="accordion-header" id="heading-${item.slabTariffId}">
                    <button class="accordion-button collapsed" type="button"
                            data-bs-toggle="collapse"
                            data-bs-target="#collapse-${item.slabTariffId}">
                        ${item.category} - ${item.subCategory} (${item.lines})
                    </button>
                </h2>
                <div id="collapse-${item.slabTariffId}" class="accordion-collapse collapse"
                     aria-labelledby="heading-${item.slabTariffId}">
                    <div class="accordion-body">
                        <a href="/SlabsAndTariffs/GetSlab?categoryId=${item.categoryId}&subCategoryId=${item.subCategoryId}&lines=${item.lines}"
                           class="btn btn-sm btn-info">Edit</a>
                    </div>
                </div>
            </div>
        `;

        container.insertAdjacentHTML("beforeend", html);
    });

    // Show correct accordion
    document.getElementById("lt-accordion").classList.toggle("d-none", accordionId !== "lt-accordion");
    document.getElementById("ht-accordion").classList.toggle("d-none", accordionId !== "ht-accordion");
}
