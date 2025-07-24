#pragma once
#include <string>
#include <vector>

// تعريف أنواع الرموز
enum class TokenType {
    KEYWORD_NUMBER, KEYWORD_TEXT, KEYWORD_PRINT, IDENTIFIER,
    NUMBER_LITERAL, STRING_LITERAL,
    EQUAL, PLUS, SEMICOLON,
    LEFT_PAREN, RIGHT_PAREN,
    END_OF_FILE, UNKNOWN
};

// تعريف هيكل الرمز
struct Token {
    TokenType type;
    std::string value;
    int line;
};

// تعريف فئة المحلل المعجمي
class Lexer {
public:
    Lexer(const std::string& source);
    std::vector<Token> tokenize();

private:
    std::string source;
    int position = 0;
    int line = 1;

    char currentChar();
    void advance();
    Token readNumber();
    Token readIdentifierOrKeyword();
};