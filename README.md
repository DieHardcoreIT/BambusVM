# BambusVM
BambusVM is a code virtualizer for .NET Framework Applications, which is based on <a href="https://github.com/hexck/Hex-Virtualization">Hex Virtualization</a>.

## For what was BambusVM developed for?
It was developed so that interested people can learn from it or get inspired by it. This VM is NOT meant for real world use. As devirtualization will be very easy compared to other VM's and there are still some bugs to fix.

## What does BambusVM do?
BambusVM converts all the code into "Bambus instructions" that only the VM or the one devirtualizing it can understand. These instructions are stored encrypted with AES and only converted when they are needed.

## What is the difference between Hex Virtualization and BambusVM?
- Dnlib has been updated to the latest version.
- The code has been simplified as much as possible.
- A secure encryption algorithm is used to protect the "Bambus instructions".
- A few OpCodes have been added. But it still does not cover all of them.

## TODO's
- [ ] Opcodes still need to be finalized.
- [ ] The VM is still kept too simple and would have to be made more complex to make devirtualization more difficult.
- [ ] Simple programs can be protected without problems, more complex programs may still have problems.

## What should I pay attention to when using it?
- Currently you can only "protect" simple programs, more complex programs will cause problems.
- I have only tested programs with the .NET framework 4.8. 
- Surely the code still contains bugs and other minor problems.

## Credits

- BambusVM is based on <a href="https://github.com/hexck/Hex-Virtualization">Hex Virtualization</a> from <a href="https://github.com/hexck">Hexk</a>.
- AES code by <a href="https://www.siakabaro.com/how-to-perform-aes-encryption-in-net/">siakabaro</a>

## License
BambusVM is licensed under the MIT License.
