echo "Building and running CodeGen"
pushd src\CodeGen
dotnet run
popd
echo "Building and packing EmojiOne NuGet package."
pushd src\EmojiOne
dotnet pack -c release
popd

