console.log("== JS Test Script Begin ==");


console.log('Hello');
console.log('Hello', true);
console.log('Hello', [true]);
console.log('A', 'B', 3, false);
console.log('A', ['B', 3, false]);

// Test window property dynamic expansion
console.log("Test window property dynamic expansion");
window.customObject.boolean = true;
window.customObject.nestedObject = { "metadata": { "some": "value" } };
console.log("Objects set");
var boolean = window.customObject.boolean;
console.log("bool Value get: " + boolean);
var value = window.customObject.nestedObject.metadata.some;
console.log("json Value get: " + value);

// Test auto-created functions are callable
console.log("Test auto-created functions are callable");
window.fakeFunc = function () { console.log("fakeFunc called!"); };
if (window.fakeFunc) {
    window.fakeFunc();
}

// Test setTimeout existence and invocation
console.log("Test windows.setTimeout invocation");
window.setTimeout(function () {
    console.log("windows.setTimeout callback executed");
}, 1000);
window.setTimeout(function (e) {
    console.log("windows.setTimeout with args callback executed: " + e);
}, 1000, "hello");


function greet(name) {
    console.log('setTimeout with args: Hello, ' + name + '!');
}
console.log("Test setTimeout with argument");
setTimeout(greet, 2000, 'Alice');



// Test atob
console.log("Test atob");
if (window.atob) {
    const encoded = "SGVsbG8sIHdvcmxk";
    const decoded = window.atob(encoded);
    console.log("atob result:" + decoded); // should be "Hello world"
}

// Test history object and methods
console.log("Test history object and methods");
if (window.history && window.history.pushState) {
    console.log("history.pushState exists");

    //history.pushState({ id: 1 }, "", "/page1");
    //console.log("history.state after pushState:" + history.state);

    //history.replaceState({ id: 2 }, "", "/page2");
    //console.log("history.state after replaceState:" + history.state);

    //history.pushState({ id: 3 }, "", "/page3");
    //console.log("history.length:" + history.length);
    //console.log("history.state (page3):" + history.state);

    //history.back();
    //console.log("history.state after back():" + history.state);

    //history.forward();
    //console.log("history.state after forward():" + history.state);

    //history.go(-2);
    //console.log("history.state after go(-2):" + history.state);
}

// Test dynamic method creation and access on window
console.log("Test dynamic method creation and access on window");
window.myFunc = function (x) {
    console.log("myFunc called with:" + x);
};
if (window.myFunc) {
    window.myFunc("test");
}

console.log("== JS Test Script End ==");