async function getData() {
    const response = await fetch("/api/notes/", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const notes = await response.json();
        const cardContainer = document.getElementById("card");
        notes.forEach(note => {
            const card = document.createElement("div");
            card.className = "card";
            const cardBody = document.createElement("div");
            cardBody.className = "card-body";
            
            const title = document.createElement("h5");
            title.className = "card-title";
            title.textContent = note.title;
            title.addEventListener("dblclick", function () {
                startEditingH5(this);
            });

            const content = document.createElement("p");
            content.className = "card-text";
            content.textContent = note.text;
            content.addEventListener("dblclick", function () {
                startEditingP(this);
            });

            const date = document.createElement("p");
            date.className = "card-date";
            date.textContent = note.changeDate;

            cardBody.appendChild(title);
            cardBody.appendChild(content);
            cardBody.appendChild(date);
            card.appendChild(cardBody);

            cardContainer.appendChild(card);
        });
    }
}
/*getData();*/

function startEditingP(element) {
    const input = document.createElement("input");
    const oldText = element.textContent;
    input.className = "inputChange inputChangeP";
    input.type = "text";
    input.value = element.textContent;
    element.replaceWith(input);
    input.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            const newText = this.value;
            const p = document.createElement("P");
            p.className = "card-title";
            p.textContent = newText;
            p.addEventListener("dblclick", function () {
                startEditingP(this);
            });
            this.replaceWith(p);
            if (oldText != newText) {
                const requestData = new FormData();
                requestData.append("noteId", p.parentNode.parentNode.id);
                requestData.append("noteEl", "Text");
                requestData.append("noteText", newText);

                fetch('/api/notes', {
                    method: "PUT",
                    body: requestData
                })
                .then(response => {
                    if (response.ok) {
                        return response.text();
                    } else {
                        console.error('Ошибка при обновлении заметки');
                    }
                })
                .then(date => {
                    p.parentNode.querySelector(".card-date").textContent = date;
                });
            }
        }
    });
    input.focus();
}
function startEditingH5(element) {
    const input = document.createElement("input");
    const oldText = element.textContent;
    input.className = "inputChange inputChangeH5";
    input.type = "text";
    input.value = element.textContent;
    element.replaceWith(input);
    input.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            const newText = this.value;
            const h = document.createElement("H5");
            h.className = "card-title";
            h.textContent = newText;
            h.addEventListener("dblclick", function () {
                startEditingH5(this);
            });
            this.replaceWith(h);
            if (oldText != newText) {
                const requestData = new FormData();
                requestData.append("noteId", h.parentNode.parentNode.id);
                requestData.append("noteEl", "Title");
                requestData.append("noteText", newText);

                fetch('/api/notes', {
                    method: "PUT",
                    body: requestData
                })
                .then(response => {
                    if (response.ok) {
                        return response.text();
                    } else {
                        console.error('Ошибка при обновлении заметки');
                    }
                })
                .then(date => {
                    h.parentNode.querySelector(".card-date").textContent = date;
                });
            }
        }
    });
    input.focus();
}

function deleteNote(element) {
    const requestData = new FormData();
    requestData.append("noteId", element.parentNode.parentNode.id);

    fetch('/api/notes', {
        method: "DELETE",
        body: requestData
    })
    .then(response => {
        if (response.ok) {
            element.parentNode.parentNode.remove();
        } else {
            console.error('Ошибка при удалении заметки');
        }
    });
}

function checkNote(element) {
    const requestData = new FormData();
    requestData.append("noteId", element.id);

    fetch('/api/notes/check', {
        method: "PUT",
        body: requestData
    })
    .then(response => {
        if (response.ok) {
            return response.text();
        } else {
            console.error('Ошибка при обновлении заметки');
        }
    })
    .then(date => {
        element.className = "card";
        element.querySelector(".card-date").textContent = date;
    });
}
