 // ملف بناء المشروع (لتسهيل الترجمة)
 # 1. تحديد الحد الأدنى من إصدار CMake المطلوب
cmake_minimum_required(VERSION 3.10)

# 2. تعريف اسم المشروع
# هذا الاسم سيستخدمه CMake داخلياً
project(FaseehCompilerProject LANGUAGES CXX)

# 3. تحديد معيار اللغة C++ المستخدم (C++17 هو خيار حديث وجيد)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 4. البحث التلقائي عن كل الملفات المصدرية (.cpp) في المجلد src
# هذا يريحك من إضافة كل ملف جديد يدوياً
file(GLOB SOURCES "src/*.cpp")

# 5. إنشاء الملف التنفيذي
# - الاسم الأول (FaseehCompiler) هو اسم الهدف، وسيكون اسم الملف التنفيذي .exe
# - ${SOURCES} هي قائمة كل الملفات .cpp التي وجدها في الخطوة السابقة
add_executable(FaseehCompiler ${SOURCES})

# رسالة اختيارية لإعلامك بمكان الملف التنفيذي عند البناء
message(STATUS "تم بناء المترجم FaseehCompiler.exe بنجاح!")