# gpstrackerd

This is a simple daemon that can be used to receive the events from a popular family of Chinesium GPS trackers and store them in a configurable backend.

Currently, the following backends are supported:

* text file
* Splunk
* KML file

The code has been tested on both MS .Net on Windows 10 and Mono on Ubuntu 20.04.

## Backends

gpstrackerd supports several different backends that can be combined to design your logging infrastructure

### Text file

Usage:

    backends:
    - backendName: TextFile
      backendEndpoint: tracker.log

Use the 'backendEndpoint' setting to configure the output file location

### Splunk 

gpstrackerd supports logging to Splunk using the HTTP event connector. The Splunk backend can be configured using the provided stanza.

Usage:

    - backendName: Splunk
      backendEndpoint: http://splunk-01.lan:8080
      authToken: 123456

### KML file 

gpstrackerd supports creating a KML file with the tracked locations. Use the `backendEndpoint` parameter to specify the output file.

Usage:

    - backendName: kml
      backendEndpoint: tracker.kml
      authToken: 123456