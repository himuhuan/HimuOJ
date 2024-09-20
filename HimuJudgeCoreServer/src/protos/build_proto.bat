protoc -I. --cpp_out=. Judge.proto
protoc -I. --grpc_out=. --plugin=protoc-gen-grpc="C:\dev\vcpkg\packages\grpc_x64-windows\tools\grpc\grpc_cpp_plugin.exe" .\Judge.proto