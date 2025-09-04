# Contributing to AspGoat

We welcome all contributions — from bug reports and code improvements to new labs and documentation.

### How to Contribute 🤝
1. **Fork & Clone**

   **Click the Fork button (top-right corner) to create your own copy under your GitHub account.**
   
   ```git clone https://github.com/<your-username>/AspGoat.git```
   
   ```cd AspGoat```

1. **Create a separate Branch**
   
   ```git checkout -b feature/add-graphql-lab```

2. **Test locally before raising a PR (using both .NET SDK and Docker)**

   ```dotnet restore```

   ```dotnet run```

   ```docker build -t aspgoat .```

   ```docker run --rm -p 8000:8000 aspgoat```

3. **Make Your Changes like the included Labs**

   - Keep code readable and commented
   - For new labs, include both vulnerable code and secure fix
   - Obfuscate the JavaScript file before committing / pushing the code with this tool : https://www.npmjs.com/package/javascript-obfuscator
  
4. **Push your code to your branch**

   ```git push origin feature/add-graphql-lab```

5. **Finally open a Pull Request (PR) to the main branch**

6. **Reporting Bugs**

   - Open an **Issue** with the **Bug** label
   - Provide Reproduction Steps
  
7. **Recognition**

   **All contributors will be credited in the Contributors section 🥳**
