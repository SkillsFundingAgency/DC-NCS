# DC-NCS
SLD and the NCS system integrate to produce payments for the area based contractors for NCS. The NCS service fabric application listens for messages, produces funding calcs and reports. It then stores the reports in a storage location that is passed back to NCS. NCS will then copy those reports to a more publicly available location. 
