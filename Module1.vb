Imports System.IO
Imports System.Text
Imports Python.Runtime

Module Module1

    Sub Main()

        Dim ppath As String = "C:\reaktoro_python"
        Dim append As String = ppath + ";" + Path.Combine(ppath, "Library", "bin") + ";"

        Dim p As String = append + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine)
        ' Set Path
        Environment.SetEnvironmentVariable("PATH", p, EnvironmentVariableTarget.Process)
        ' Set PythonHome
        Environment.SetEnvironmentVariable("PYTHONHOME", ppath, EnvironmentVariableTarget.Process)
        ' Set PythonPath
        Environment.SetEnvironmentVariable("PYTHONPATH", Path.Combine(p, "Lib"), EnvironmentVariableTarget.Process)

        Using (Py.GIL())

            Dim reaktoro As Object = Py.Import("reaktoro")

            '' Step 2: Initialize a thermodynamic database
            Dim db = reaktoro.Database("supcrt98.xml")

            '' Step 3: Define the chemical system
            Dim editor = reaktoro.ChemicalEditor(db)
            editor.addAqueousPhase("H2O(l) H+ OH- HCO3- CO2(aq) NaCl(aq) Na+ Cl-")
            editor.addGaseousPhase("CO2(g)")

            '' Step 4: Construct the chemical system
            Dim mySystem = reaktoro.ChemicalSystem(editor)

            '' Step 5: Define the chemical equilibrium problem
            Dim problem = reaktoro.EquilibriumProblem(mySystem)
            problem.setTemperature(60, "celsius")
            problem.setPressure(100, "bar")
            problem.add("H2O", 1.0, "kg")
            problem.add("NaCl", 1.0, "mol")
            problem.add("Na+", 1.0, "mol")
            problem.add("CO2", 10.0, "mol")

            ' Step 6: Calculate the chemical equilibrium state
            Dim state = reaktoro.equilibrate(problem)

            Dim properties = state.properties

            Dim species = mySystem.species()

            For Each item In species
                Console.WriteLine(item.name)
            Next

            Dim phases = mySystem.phases()

            Console.WriteLine(state.phaseAmount("Aqueous"))
            Console.WriteLine(state.phaseAmount("Gaseous"))

            For Each item In phases
                Console.WriteLine(item.name)
            Next

            Dim amounts = state.speciesAmounts()

            For Each item In amounts
                Console.WriteLine(item)
            Next

            Dim ac = properties.lnActivityCoefficients().val

            For Each item In ac
                Console.WriteLine(item)
            Next

        End Using

    End Sub

End Module
