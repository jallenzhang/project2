//	HYPE.documents["FrontPage"]

(function HYPE_DocumentLoader() {
	var resourcesFolderName = "FrontPage_Resources";
	var documentName = "FrontPage";
	var documentLoaderFilename = "frontpage_hype_generated_script.js";

	// find the URL for this script's absolute path and set as the resourceFolderName
	try {
		var scripts = document.getElementsByTagName('script');
		for(var i = 0; i < scripts.length; i++) {
			var scriptSrc = scripts[i].src;
			if(scriptSrc != null && scriptSrc.indexOf(documentLoaderFilename) != -1) {
				resourcesFolderName = scriptSrc.substr(0, scriptSrc.lastIndexOf("/"));
				break;
			}
		}
	} catch(err) {	}

	// Legacy support
	if (typeof window.HYPE_DocumentsToLoad == "undefined") {
		window.HYPE_DocumentsToLoad = new Array();
	}
 
	// load HYPE.js if it hasn't been loaded yet
	if(typeof HYPE_100 == "undefined") {
		if(typeof window.HYPE_100_DocumentsToLoad == "undefined") {
			window.HYPE_100_DocumentsToLoad = new Array();
			window.HYPE_100_DocumentsToLoad.push(HYPE_DocumentLoader);

			var headElement = document.getElementsByTagName('head')[0];
			var scriptElement = document.createElement('script');
			scriptElement.type= 'text/javascript';
			scriptElement.src = resourcesFolderName + '/' + 'HYPE.js?hype_version=100';
			headElement.appendChild(scriptElement);
		} else {
			window.HYPE_100_DocumentsToLoad.push(HYPE_DocumentLoader);
		}
		return;
	}
	
	var hypeDoc = new HYPE_100();
	
	var attributeTransformerMapping = {b:"i",c:"i",bC:"i",d:"i",aS:"i",M:"i",e:"f",N:"i",f:"d",aT:"i",O:"i",g:"c",aU:"i",P:"i",Q:"i",aV:"i",R:"c",aW:"f",aI:"i",S:"i",T:"i",l:"d",aX:"i",aJ:"i",m:"c",n:"c",aK:"i",X:"i",aZ:"i",A:"c",Y:"i",aL:"i",B:"c",C:"c",D:"c",t:"i",E:"i",G:"c",bA:"c",a:"i",bB:"i"};

	var scenes = [{ onSceneUnloadAction: { type: 0 }, timelines: { "28_pressed": { framesPerSecond: 30, animations: [], identifier: "28_pressed", name: "28_pressed", duration: 0 }, kTimelineDefaultIdentifier: { framesPerSecond: 30, animations: [{ f: "2", t: 0, d: 0.36666667, i: "a", e: -14.768393, r: 1, s: 311, o: "26" }, { f: "2", t: 0, d: 0.36666667, i: "c", e: 729.76839, r: 1, s: 79, o: "26" }, { f: "2", t: 0, d: 0.36666667, i: "d", e: 841, r: 1, s: 91, o: "26" }, { f: "2", t: 0, d: 0.36666667, i: "b", e: -77, r: 1, s: 280, o: "26" }, { f: "2", t: 0, d: 0.36666667, i: "e", e: "1.000000", r: 1, s: "0.231607", o: "26" }, { f: "2", t: 0.36666667, d: 0.16666669, i: "e", e: "1.231607", s: "1.000000", o: "26" }, { f: "2", t: 0.36666667, d: 0.16666669, i: "d", e: 684, s: 841, o: "26" }, { f: "2", t: 0.36666667, d: 0.16666669, i: "b", e: 8, s: -77, o: "26" }, { f: "2", t: 0.36666667, d: 0.16666669, i: "c", e: 594, s: 729.76839, o: "26" }, { f: "2", t: 0.36666667, d: 0.16666669, i: "a", e: 53, s: -14.768393, o: "26" }, { f: "2", t: 0.53333336, d: 0.19999999, i: "d", e: 700, s: 684, o: "26" }, { f: "2", t: 0.53333336, d: 0.19999999, i: "c", e: 608, s: 594, o: "26" }, { f: "2", t: 0.53333336, d: 0.19999999, i: "e", e: "1.231607", s: "1.231607", o: "26" }, { f: "2", t: 0.53333336, d: 0.19999999, i: "a", e: 46, s: 53, o: "26" }, { f: "2", t: 0.53333336, d: 0.19999999, i: "b", e: 0, s: 8, o: "26"}], identifier: "kTimelineDefaultIdentifier", name: "Main Timeline", duration: 0.73333335} }, sceneIndex: 0, perspective: "600px", oid: "8", initialValues: { "26": { o: "content-box", h: "logo.png", p: "no-repeat", x: "hidden", a: 311, q: "100% 100%", b: 280, j: "absolute", r: "inline", aX: 0, c: 79, k: "div", d: 91, z: "8", e: "0.231607"} }, background: "transparent", name: "frontPage"}];


	
	var javascripts = [];


	
	var Custom = {};
	var javascriptMapping = {};
	for(var i = 0; i < javascripts.length; i++) {
		try {
			javascriptMapping[javascripts[i].identifier] = javascripts[i].name;
			eval("Custom." + javascripts[i].name + " = " + javascripts[i].source);
		} catch (e) {
			hypeDoc.log(e);
			Custom[javascripts[i].name] = (function () {});
		}
	}
	
	hypeDoc.setAttributeTransformerMapping(attributeTransformerMapping);
	hypeDoc.setScenes(scenes);
	hypeDoc.setJavascriptMapping(javascriptMapping);
	hypeDoc.Custom = Custom;
	hypeDoc.setCurrentSceneIndex(0);
	hypeDoc.setMainContentContainerID("frontpage_hype_container");
	hypeDoc.setResourcesFolderName(resourcesFolderName);
	hypeDoc.setShowHypeBuiltWatermark(0);
	hypeDoc.setShowLoadingPage(false);
	hypeDoc.setDrawSceneBackgrounds(true);
	hypeDoc.setDocumentName(documentName);

	HYPE.documents[documentName] = hypeDoc.API;

	hypeDoc.documentLoad(this.body);
}());

