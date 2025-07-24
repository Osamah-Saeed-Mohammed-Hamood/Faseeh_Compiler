#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include "lexer.h"

// دالة لتحويل نوع الرمز إلى نص يمكن طباعته
std::string tokenTypeToString(TokenType type) {
    switch (type) {
    case TokenType::KEYWORD_NUMBER: return "KEYWORD_NUMBER";
    case TokenType::KEYWORD_PRINT: return "KEYWORD_PRINT";
    case TokenType::IDENTIFIER: return "IDENTIFIER";
    case TokenType::NUMBER_LITERAL: return "NUMBER_LITERAL";
    case TokenType::EQUAL: return "EQUAL";
    case TokenType::SEMICOLON: return "SEMICOLON";
    case TokenType::END_OF_FILE: return "END_OF_FILE";
    default: return "UNKNOWN";
    }
}

int main(int argc, char* argv[]) {
    if (argc < 2) {
        std::cerr << "خطأ: لم يتم تحديد ملف مصدري." << std::endl;
        return 1;
    }

    std::ifstream file(argv[1]);
    if (!file.is_open()) {
        std::cerr << "خطأ: لا يمكن فتح الملف " << argv[1] << std::endl;
        return 1;
    }

    std::stringstream buffer;
    buffer << file.rdbuf();
    std::string sourceCode = buffer.str();

    Lexer lexer(sourceCode);
    std::vector<Token> tokens = lexer.tokenize();

    for (const auto& token : tokens) {
        std::cout << "Type: " << tokenTypeToString(token.type)
            << ", Value: '" << token.value
            << "', Line: " << token.line << std::endl;
    }

    return 0;
}