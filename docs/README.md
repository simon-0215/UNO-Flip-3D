<Please keep the folder structure as given in the template repo. We will discuss each artifact as we get to it in the course. In some cases, like for the SRS, you should have a file of the same name. For other cases, like the design documentation, you are required to document your design, but it may not be via a module guide and module interface specification documents.>

<The files and folders have been set-up with tex files that have external links so that cross-referencing is possible between documents.>

<The tex files use Common.tex so that they can share definitions.>

<The files use Comments.tex so that the comments package can be used to embed comments into the generated PDF. Comments can be set to false so that they do not appear.>

<None of the files are complete templates. You will need to add extra information. They are just intended to be a starting point.>

<You should select an SRS template. Three options are available in the repo, or you can introduce another template. You should delete any SRS options that you do not need. The folder `SRS` holds a template for Scientific Computing software; the folder `SRS-Volere` holds the Volere template in LaTeX; the folder `SRS-Meyer` holds the template that Dr. Mosser now uses in the third-year requirements course.>

<The Makefile assumes the SRS will be in a folder called `SRS`. If you use the Makefile with a template other than the Scientific Computing template, you will have to delete the unnecessary folders and rename your folder to `SRS`.>

# Documentation folders

The folders and files for this folder are as follows:

<pre>
UNO-Flip-3D/
│
├── docs/                        # All documentation for the project
│   ├── Design/
│   │   ├── SoftArchitecture/    # Module Guide (MG)
│   │   │   └── MG.pdf
│   │   ├── SoftDetailedDes/     # Module Interface Specification (MIS)
│   │   │   └── MIS.pdf
│   ├── DevelopmentPlan/         # Gantt chart, timeline, and task breakdown
│   │   └── DevelopmentPlan.pdf
│   ├── HazardAnalysis/          # Addresses hazards within the scope of the project
│   │   └── HazardAnalysis.pdf
│   ├── Presentations/           # Demos and EXPO presentation materials
│   │   ├── D0_ProofOfConceptDemo/
│   │   ├── D1_Rev0Demo/
│   │   │   └── Uno Flip.pptm
│   │   ├── D2_FinalPresentation/
│   │   │   ├── D2_FinalPresentation.pdf
│   │   │   ├── Script.pdf
│   │   │   └── UNO-Flip-Remix-Revision-1-Demo.pptx
│   │   └── D3_EXPO/
│   │       └── expo.pdf
│   ├── ProblemStatementAndGoals/    # Specifies problem statement and goals of project
│   │   └── ProblemStatement.pdf
│   ├── ReflectAndTrace/         # Traceability of addressed feedback
│   │   └── ReflectAndTrace.pdf
│   ├── SRS-Volere/              # Software Requirements Specification
│   │   └── SRS.pdf
│   ├── UserGuide/               # User Manual for setup
│   │   └── UserGuide.pdf
│   ├── VnVPlan/                 # Testing plan
│   │   └── VnVPlan.pdf
│   ├── VnVReport/               # Testing report
│   │   └── VnVReport.pdf
│   └── Extras/                  # Includes 2 extras
│       ├── Usability Testing/
│       │   └── UsabilityTesting.pdf
│       └── GenderMAG Personas/
│           └── GenderMAG.pdf
│
├── refs/                        # External references (e.g., UNO rules, research)
│
├── UNOFlip/                     # Unity project source code
│   ├── Assets/                  # Game scenes, scripts, UI, and prefabs
│   ├── Packages/
│   └── ProjectSettings/
│
└── Network UNO Card Game TCP Server/  # TCP server for multiplayer game sessions
</pre>
